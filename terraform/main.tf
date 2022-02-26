terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.27"
    }
  }

  required_version = ">= 0.14.9"

  backend "s3" {
    bucket = "terraform-state-cloudstore"
    key    = "terraform/DocumentService/terraform.tfstate"
    region = "eu-west-1"
  }
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

