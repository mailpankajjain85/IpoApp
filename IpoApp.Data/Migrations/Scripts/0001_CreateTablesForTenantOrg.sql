CREATE TABLE TenantOrg (
    OrgID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrgName VARCHAR(255) NOT NULL,
    OrgShortCode VARCHAR(50) NOT NULL UNIQUE,
    OrgPhoneNumber VARCHAR(20),
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Add a comment for the table
COMMENT ON TABLE TenantOrg IS 'Table storing tenant organization information';

-- Add comments for columns
COMMENT ON COLUMN TenantOrg.OrgID IS 'Primary key, UUID identifier for the organization';
COMMENT ON COLUMN TenantOrg.OrgName IS 'Full name of the organization';
COMMENT ON COLUMN TenantOrg.OrgShortCode IS 'Short code or abbreviation for the organization';
COMMENT ON COLUMN TenantOrg.OrgPhoneNumber IS 'Primary contact phone number for the organization';
COMMENT ON COLUMN TenantOrg.CreatedAt IS 'Timestamp when the record was created';
COMMENT ON COLUMN TenantOrg.UpdatedAt IS 'Timestamp when the record was last updated';

-- Create an index on the short code for faster lookups
CREATE INDEX idx_tenantorg_shortcode ON TenantOrg(OrgShortCode);

-- Create a trigger to automatically update the UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_tenantorg_updatedat()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_tenantorg_updatedat
BEFORE UPDATE ON TenantOrg
FOR EACH ROW
EXECUTE FUNCTION update_tenantorg_updatedat();


