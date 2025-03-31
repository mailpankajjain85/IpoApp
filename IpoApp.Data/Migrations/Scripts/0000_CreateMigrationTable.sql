DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'appliedmigrations') THEN
        CREATE TABLE AppliedMigrations (
            MigrationId VARCHAR(50) PRIMARY KEY,
            AppliedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
    END IF;
END $$;