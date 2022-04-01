resource "aws_elasticache_cluster" "redis" {
  cluster_id           = "redis-cache"
  engine               = "redis"
  node_type            = "cache.m2.micro"
  num_cache_nodes      = 1
  parameter_group_name = "default.redis3.2"
  engine_version       = "3.2.10"
  port                 = 6379
}