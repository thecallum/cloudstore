resource "aws_iam_role" "sns_feedback_role" {
    name = "sns_feedback_role"

    assume_role_policy = jsonencode({
        "Version": "2012-10-17",
        "Statement": [
            {
                "Sid": "Stmt1646404052824",
                "Action": "sts:AssumeRole",
                "Effect": "Allow",
                # "Resource": "*",
                  "Principal": {
                    "Service": [
                    "sns.amazonaws.com"
                    ]
                },
            }
        ]
    })
}

resource "aws_iam_policy" "sns_feedback_policy" {
  name        = "sns_feedback_policy"
  description = "Give SNS topic within CloudStore permission to create logs"

  policy = jsonencode({
        "Version": "2012-10-17",
        "Statement": [
            {
            "Sid": "Stmt1646404052824",
            "Action": [
                "logs:CreateLogGroup",
                "logs:CreateLogStream",
                "logs:PutLogEvents",
                "logs:PutMetricFilter",
                "logs:PutRetentionPolicy"
            ],
            "Effect": "Allow",
            "Resource": "*"
            }
        ]
    })
}

resource "aws_iam_role_policy_attachment" "sns_feedback_policy" {
   role       = "${aws_iam_role.sns_feedback_role.name}"
   policy_arn = "${aws_iam_policy.sns_feedback_policy.arn}"
}
