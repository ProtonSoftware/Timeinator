CREATE DATABASE "db_timeinator";
USE "db_timeinator";

DROP TABLE IF EXISTS "users";
CREATE TABLE "users" (
    "id" int NOT NULL UNIQUE,
    "email" VARCHAR(255) NOT NULL UNIQUE,
    "password" VARCHAR(255) NOT NULL,
    "salt" VARCHAR(255) NOT NULL,
    "token" VARCHAR(255) DEFAULT NULL,
    "token_expiration" TIMESTAMP DEFAULT NULL,
    PRIMARY KEY ("id")
);

DROP TABLE IF EXISTS "tasks";
CREATE TABLE "tasks" (
    "id" int NOT NULL UNIQUE,
    "user" int NOT NULL,
    "active" boolean DEFAULT 0,
    "time" TIMESTAMP NOT NULL,
    "duration" TIME DEFAULT NULL,
    "priority" int NOT NULL,
    "cycle" TIME DEFAULT NULL,
    "progress" FLOAT DEFAULT 0,
    "started" boolean DEFAULT 0,
    PRIMARY KEY ("id")
);

DROP TABLE IF EXISTS "freetime";
CREATE TABLE "freetime" (
    "id" int NOT NULL UNIQUE,
    "user" int NOT NULL,
    "begin" TIMESTAMP NOT NULL,
    "end" TIMESTAMP NOT NULL,
    PRIMARY KEY ("id")
);