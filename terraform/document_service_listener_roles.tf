resource "aws_iam_role" "document_service_listener_role" {
  name = "document_service_listener_role"

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

resource "aws_iam_role_policy_attachment" "b" {
   role       = "${aws_iam_role.document_service_listener_role.name}"
   policy_arn = "${data.aws_iam_policy.AWSLambdaBasicExecutionRole.arn}"
}

resource "aws_iam_policy" "document_service_listener_s3_access" {
  name        = "document_service_listener_s3_access"
  description = "Give DocumentService within CloudStore permission to access S3Bucket"

  policy = jsonencode({
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "Stmt1646400709921",
            "Action": [
                "s3:DeleteObject",
                "s3:DeleteObjectTagging",
                "s3:GetObject",
                "s3:GetObjectAcl",
                "s3:GetObjectTagging",
                "s3:PutObject",
                "s3:PutObjectTagging"
            ],
            "Effect": "Allow",
            "Resource": "${aws_s3_bucket.document_storage.arn}/*",
        }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "e" {
   role       = "${aws_iam_role.document_service_listener_role.name}"
   policy_arn = "${aws_iam_policy.document_service_listener_s3_access.arn}"
}

resource "aws_iam_role_policy_attachment" "AWSLambdaSQSQueueExecutionRole" {
   role       = "${aws_iam_role.document_service_listener_role.name}"
   policy_arn = "${data.aws_iam_policy.AWSLambdaSQSQueueExecutionRole.arn}"
}

resource "aws_security_group" "document_service_listener_security_group" {
  name        = "DocumentServiceListener security group"
  description = "DocumentServiceListener security Group"

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