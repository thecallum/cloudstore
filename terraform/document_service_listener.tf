data "aws_s3_bucket_object" "document_service_listener_hash" {
  bucket = "terraform-state-cloudstore"
  key    = "/DocumentServiceListener/DocumentServiceListener.zip.hash"
}

resource "aws_lambda_function" "DocumentServiceListener" {
  function_name = "DocumentServiceListener"
  role = aws_iam_role.document_service_listener_role.arn
  handler = "DocumentServiceListener::DocumentServiceListener.LambdaEntryPoint::FunctionHandler"
  runtime = "dotnetcore3.1"

  s3_bucket = "terraform-state-cloudstore"
  s3_key = "DocumentServiceListener/DocumentServiceListener.zip"

  description = "Listener endpoint for CloudStore"

  timeout = 300
  memory_size = 256

  source_code_hash = data.aws_s3_bucket_object.document_service_listener_hash.body

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      CONNECTION_STRING = var.DatabaseConnectionString
      S3_BUCKET_NAME = aws_s3_bucket.document_storage.bucket
      S3_BUCKET_BASE_PATH = "https://${ aws_s3_bucket.document_storage.bucket}.s3.${data.aws_region.current.name}.amazonaws.com/thumbnails"
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