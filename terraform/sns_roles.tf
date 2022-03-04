resource "aws_iam_role" "sns_feedback_role" {
  name = "sns_feedback_role"

  assume_role_policy = jsonencode({
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "logs:CreateLogGroup",
                "logs:CreateLogStream",
                "logs:PutLogEvents",
                "logs:PutMetricFilter",
                "logs:PutRetentionPolicy"
            ],
            "Resource": "*"
        }
    ]
  })
}

# resource "aws_iam_role" "sns_success_feedback_role" {
#   name = "sns_success_feedback_role"

#   assume_role_policy = jsonencode({
#     "Version": "2012-10-17",
#     "Statement": [
#         {
#         "Effect": "Allow",
#         "Action": [
#             "logs:CreateLogGroup",
#             "logs:CreateLogStream",
#             "logs:PutLogEvents",
#             "logs:PutMetricFilter",
#             "logs:PutRetentionPolicy"
#         ],
#         "Resource": [
#             "*"
#         ]
#         }
#     ]
#   })
# }