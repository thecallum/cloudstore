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

resource "aws_iam_policy" "document_service_s3_access" {
  name        = "document_service_s3_access"
  description = "Give DocumentService within CloudStore permission to access S3Bucket"

  # Terraform's "jsonencode" function converts a
  # Terraform expression result to valid JSON syntax.
  policy = jsonencode({
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "s3:PutObject",
                "s3:GetObjectAcl",
                "s3:GetObject",
                "s3:GetObjectTagging",
                "s3:PutObjectTagging",
                "s3:DeleteObject"
            ],
            "Resource": "${aws_s3_bucket.document_storage.arn}/*",
        }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "c" {
   role       = "${aws_iam_role.document_service_role.name}"
   policy_arn = "${aws_iam_policy.document_service_s3_access.arn}"
}

resource "aws_iam_policy" "document_service_sns_publish" {
  name        = "document_service_sns_publish"
  description = "Give DocumentService within CloudStore permission to publish to SNS event"
  policy = jsonencode({
    "Version": "2012-10-17",
    "Statement": [
      {
        "Sid":"AllowPublishToMyTopic",
        "Effect":"Allow",
        "Action":"sns:Publish",
        "Resource":"arn:aws:sns:us-east-2:123456789012:MyTopic",
        "Resource": aws_sns_topic.DocumentService.arn
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "d" {
   role       = "${aws_iam_role.document_service_role.name}"
   policy_arn = "${aws_iam_policy.document_service_sns_publish.arn}"
}

resource "aws_iam_role_policy_attachment" "a" {
   role       = "${aws_iam_role.document_service_role.name}"
   policy_arn = "${data.aws_iam_policy.AWSLambdaBasicExecutionRole.arn}"
}

resource "aws_security_group" "document_service_security_group" {
  name        = "document_service_security_group"
  description = "DocumentService security Group"

  ingress {
    description      = "Temp Allow All"
    protocol         = "tcp"
    from_port        = "0"
    to_port          = "6553"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    description      = "Temp Allow All"
    protocol         = "tcp"
    from_port        = "0"
    to_port          = "6553"
    cidr_blocks = ["0.0.0.0/0"]
  }
}