resource "aws_security_group" "redis_security_group" {
  name        = "Redis Security Group"
  description = "RedisCache security Group"

  ingress {
    description      = "Allow access from DocumentService"
    protocol         = "tcp"
    from_port        = "6379"
    to_port          = "6379"
    # cidr_blocks      = ["0.0.0.0/0"]
    source_security_group_id = aws_security_group.document_service_security_group.id
  }

  egress {
    description      = "Allow output to any service"
    protocol         = "tcp"
    from_port        = "6379"
    to_port          = "6379"
    cidr_blocks = ["0.0.0.0/0"]
  }
}