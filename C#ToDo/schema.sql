USE todo_db;

CREATE TABLE IF NOT EXISTS tasks (
    id VARCHAR(36) PRIMARY KEY NOT NULL,
    description VARCHAR(255) NOT NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    importance INT NOT NULL,
    date_added DATETIME NOT NULL
);
