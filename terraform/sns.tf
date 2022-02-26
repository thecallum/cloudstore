resource "aws_sns_topic" "DocumentService" {
  name                        = "DocumentService.fifo"
  fifo_topic                  = true
  content_based_deduplication = true
}

resource "aws_sqs_queue" "DocumentService" {
  name                        = "DocumentService.fifo"
  fifo_queue                  = true
  content_based_deduplication = true

  delay_seconds             = 0
  max_message_size          = 262144 
  message_retention_seconds = 86400 # 1 day
  receive_wait_time_seconds = 0
  visibility_timeout_seconds = 300
 
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.DocumentServiceDLQ.arn
    maxReceiveCount     = 2
  })
}

resource "aws_sqs_queue" "DocumentServiceDLQ" {
  name                        = "DocumentServiceDLQ.fifo"
  fifo_queue                  = true

  delay_seconds             = 0
  max_message_size          = 262144 
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 0
}

resource "aws_sqs_queue_policy" "test" {
  queue_url = aws_sqs_queue.DocumentService.id

  policy = <<POLICY
{
  "Version": "2008-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "sns.amazonaws.com"
      },
      "Action": "sqs:SendMessage",
      "Resource": "${aws_sqs_queue.DocumentService.arn}",
      "Condition": {
        "ArnEquals": {
          "aws:SourceArn": "${aws_sns_topic.DocumentService.arn}"
        }
      }
    }
  ]
}
POLICY
}

#


resource "aws_sns_topic_subscription" "DocumentServiceSubsciption" {
  topic_arn = aws_sns_topic.DocumentService.arn
  protocol  = "sqs"
  endpoint  = aws_sqs_queue.DocumentService.arn
}