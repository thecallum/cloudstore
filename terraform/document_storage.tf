resource "aws_s3_bucket" "document_storage" {
  bucket = "cloudstore-document-storage"
  acl    = "public-read"

  tags = {
    Name        = "CloudStoreDocumentStorage"
    Environment = "Prod"
  }

  lifecycle_rule {
    id      = "uploadDelete"
    enabled = true

    prefix = "upload/"

    expiration {
      days = 1
    }
  }

  cors_rule {
    allowed_headers = ["*"]
    allowed_methods = ["PUT", "POST", "GET"]
    allowed_origins = ["*"]
    expose_headers  = []
    max_age_seconds = 3000
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
