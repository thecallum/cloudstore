resource "aws_lambda_function" "DocumentServiceListener" {
  function_name = "DocumentServiceListener"
  role = "arn:aws:iam::714664911966:role/DocumentServiceListenerRole"
  handler = "DocumentServiceListener::DocumentServiceListener.LambdaEntryPoint::FunctionHandler"
  runtime = "dotnetcore3.1"

  s3_bucket = "terraform-state-cloudstore"
  s3_key = "DocumentServiceListener/DocumentServiceListener.zip"

  description = "description..."

  timeout = 300
  memory_size = 256

  vpc_config {
    subnet_ids         = ["subnet-a4ce21ef", "subnet-96e4f8f0", "subnet-2b441d71"]
    security_group_ids = ["sg-001484f2556e719ec", "sg-fb8b0daa"]
  }

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      DatabaseConnectionString = var.DatabaseConnectionString
    }
  }
}

# SQS trigger
resource "aws_lambda_event_source_mapping" "event_source_mapping" {
  event_source_arn = aws_sqs_queue.DocumentService.arn
  enabled          = true
  function_name    = aws_lambda_function.DocumentServiceListener.arn
  batch_size       = 10
}