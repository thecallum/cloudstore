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

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      CONNECTION_STRING = var.DatabaseConnectionString
      S3_BUCKET_BASE_PATH = "https://${aws_s3_bucket.S3.bucket}.s3.${data.aws_region.current}.amazonaws.com/thumbnails"
      
      

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