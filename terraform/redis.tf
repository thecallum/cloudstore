resource "aws_elasticache_cluster" "redis" {
  cluster_id           = "redis-cache"
  engine               = "redis"
  node_type            = "cache.t2.micro"
  num_cache_nodes      = 2
  parameter_group_name = "default.redis3.2"
  engine_version       = "3.2.10"
  port                 = 6379
  security_group_ids   = []
}

# resource "aws_security_group" "redis_security_group" {
#   name        = "redis_security_group"
#   description = "RedisCache security Group"

#   ingress {
#     description      = "Temp Allow All"
#     protocol         = "tcp"
#     from_port        = "0"
#     to_port          = "6553"
#     cidr_blocks = ["0.0.0.0/0"]
#   }

#   egress {
#     description      = "Temp Allow All"
#     protocol         = "tcp"
#     from_port        = "0"
#     to_port          = "6553"
#     cidr_blocks = ["0.0.0.0/0"]
#   }
# }