terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.27"
    }
  }

  required_version = ">= 0.14.9"
}

provider "aws" {
  region  = "eu-west-1"
}

variable "DatabaseConnectionString" {
    type = string
}

variable "SECRET" {
    type = string
}


resource "aws_lambda_function" "example" {
  function_name = "example-DocumentService2"
  role = "arn:aws:iam::714664911966:role/DocumentServiceRole"
  handler = "DocumentService::DocumentService.LambdaEntryPoint::FunctionHandlerAsync"
  runtime = "dotnetcore3.1"

  s3_bucket = "terraform-state-cloudstore"
  s3_key = "DocumentService/DocumentService.zip"

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
      SECRET = var.SECRET
      SNS_TOPIC_ARN = "arn:aws:sns:eu-west-1:714664911966:DocumentService.fifo"
    }
  }
}

resource "aws_lambda_permission" "apigw" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.example.function_name}"
  principal     = "apigateway.amazonaws.com"

  # The /*/* portion grants access from any method on any resource
  # within the API Gateway "REST API".
  source_arn = "${aws_api_gateway_rest_api.example.execution_arn}/*/*/*"
}