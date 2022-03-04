data "aws_s3_bucket_object" "document_service_hash" {
  bucket = "terraform-state-cloudstore"
  key    = "/DocumentService/DocumentService.zip.hash"
}

resource "aws_lambda_function" "DocumentService" {
  function_name = "DocumentService"
  #role = "arn:aws:iam::714664911966:role/DocumentServiceRole"
  role = aws_iam_role.document_service_role.arn
  handler = "DocumentService::DocumentService.LambdaEntryPoint::FunctionHandlerAsync"
  runtime = "dotnetcore3.1"

  s3_bucket = "terraform-state-cloudstore"
  s3_key = "DocumentService/DocumentService.zip"

  description = "description..."

  timeout = 300
  memory_size = 256

  source_code_hash = data.aws_s3_bucket_object.document_service_hash.body

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      DatabaseConnectionString = var.DatabaseConnectionString
      SECRET = var.SECRET
      SNS_TOPIC_ARN = aws_sns_topic.DocumentService.arn
      S3_BUCKET_NAME = aws_s3_bucket.document_storage.bucket
    }
  }
}

resource "aws_iam_role" "document_service_role" {
  name = "document_service_role"

  assume_role_policy = jsonencode({
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "sts:AssumeRole"
            ],
            "Principal": {
                "Service": [
                    "lambda.amazonaws.com"
                ]
            }
        }
    ]
  })
}

data "aws_iam_policy" "AWSLambdaBasicExecutionRole" {
  arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role_policy_attachment" "AWSLambdaBasicExecutionRole" {
   role       = "${aws_iam_role.document_service_role.name}"
   policy_arn = "${data.aws_iam_policy.AWSLambdaBasicExecutionRole.arn}"
}

resource "aws_lambda_permission" "apigw" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.DocumentService.function_name}"
  principal     = "apigateway.amazonaws.com"

  # The /*/* portion grants access from any method on any resource
  # within the API Gateway "REST API".
  source_arn = "${aws_api_gateway_rest_api.apiGateway.execution_arn}/*/*/*"
}