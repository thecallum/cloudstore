data "aws_iam_policy" "AWSLambdaBasicExecutionRole" {
  arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
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
      },
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
        "Resource": "${aws_s3_bucket.document_storage.arn}/*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "a" {
   role       = "${aws_iam_role.document_service_role.name}"
   policy_arn = "${data.aws_iam_policy.AWSLambdaBasicExecutionRole.arn}"
}

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

