
resource "aws_vpc_endpoint" "s3" {
  vpc_id       = "vpc-81eb49f8"
  service_name = "com.amazonaws.eu-west-1.s3"

  tags = {
    Name = "S3 VPC Endpoint"
  }
}

resource "aws_vpc_endpoint" "sns" {
  vpc_id            = "vpc-81eb49f8"
  service_name      = "com.amazonaws.eu-west-1.sns"
  vpc_endpoint_type = "Interface"

  security_group_ids = [
    aws_security_group.sns_vpc_endpoint.id,
  ]

  route_table_ids = [
    "rtb-5933e621"
  ]

  private_dns_enabled = true

  tags = {
    Name = "SNS VPC Endpoint"
  }
}

resource "aws_security_group" "sns_vpc_endpoint" {
  name        = "SNS VPC Endpoint"
  description = "Allow services to publish to SNS"

  ingress {
    protocol         = "tcp"
    from_port        = "0"
    to_port          = "65535"
    cidr_blocks      = ["0.0.0.0/0"]
  }

  egress {
    protocol         = "tcp"
    from_port        = "0"
    to_port          = "65535"
    cidr_blocks      = ["0.0.0.0/0"]
  }
}