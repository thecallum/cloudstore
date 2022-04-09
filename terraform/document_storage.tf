resource "aws_s3_bucket" "document_storage" {
  bucket = "cloudstore-document-storage"

  tags = {
    Name        = "CloudStoreDocumentStorage"
    Environment = "Prod"
  }
}

resource "aws_s3_bucket_policy" "allow_thumbnail_get" {
  bucket = aws_s3_bucket.document_storage.id
  policy = data.aws_iam_policy_document.allow_thumbnail_get.json
}

data "aws_iam_policy_document" "allow_thumbnail_get" {
  statement {
    principals {
      type        = "AWS"
      identifiers = ["*"]
    }

    actions = [
      "s3:GetObject",
    ]

    resources = [
      "${aws_s3_bucket.document_storage.arn}/thumbnails/*",
    ]
  }
}

resource "aws_s3_bucket_acl" "document_storage_acl" {
  bucket = aws_s3_bucket.document_storage.id
  acl    = "public-read"
}

resource "aws_s3_bucket_cors_configuration" "document_storage_cors" {
  bucket = aws_s3_bucket.document_storage.bucket

  cors_rule {
    allowed_headers = ["*"]
    allowed_methods = ["PUT", "POST", "GET"]
    allowed_origins = ["*"]
    expose_headers  = []
    max_age_seconds = 3000
  }
}

resource "aws_s3_bucket_lifecycle_configuration" "document_storage_lifecycle" {
  bucket = aws_s3_bucket.document_storage.id

  rule {
    id      = "uploadDelete"
    status = "Enabled"

    filter {
      prefix = "upload/"
    }

    expiration {
      days = 1
    }
  }
}