-- 1. Create IpoType Master Table (only TypeName as primary key)
CREATE TABLE IpoType (
    TypeName VARCHAR(20) PRIMARY KEY,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert predefined values
INSERT INTO IpoType (TypeName) VALUES 
('SME'),
('Mainline');

-- 2. Create IpoAppType Master Table (only AppType as primary key)
CREATE TABLE IpoAppType (
    AppType VARCHAR(20) PRIMARY KEY,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert predefined values
INSERT INTO IpoAppType (AppType) VALUES 
('Retail'),
('SHNI'),
('BHNI'),
('HNI');

-- 3. Create SaudaType Master Table (only TypeName as primary key)
CREATE TABLE SaudaType (
    TypeName VARCHAR(20) PRIMARY KEY,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert predefined values
INSERT INTO SaudaType (TypeName) VALUES 
('App'),
('Shares'),
('SubjectTo');

-- Recreate IpoMaster table with proper foreign key references
CREATE TABLE IpoMaster (
    ID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(255) NOT NULL,
    OrgShortCode VARCHAR(50) NOT NULL,
    ClosingDate DATE NOT NULL,
    ListingDate DATE NOT NULL,
    Registrar VARCHAR(100),
    IPOType VARCHAR(20) NOT NULL REFERENCES IpoType(TypeName),
    HisabDone BOOLEAN DEFAULT FALSE,
    CreatedDate TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(50) NOT NULL,
    UpdatedDate TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(50),
    
    -- Foreign key to TenantOrg (assuming it exists)
    CONSTRAINT fk_ipo_tenant FOREIGN KEY (OrgShortCode) 
    REFERENCES TenantOrg(OrgShortCode)
);

-- Add comments
COMMENT ON TABLE IpoMaster IS 'Master table for IPO information';
COMMENT ON COLUMN IpoMaster.IPOType IS 'Type of IPO (references IpoType.TypeName)';

-- Create indexes for performance
CREATE INDEX idx_ipomaster_ipo_type ON IpoMaster(IPOType);
CREATE INDEX idx_ipomaster_dates ON IpoMaster(ClosingDate, ListingDate);

-- Create trigger for auto-updating timestamp
CREATE OR REPLACE FUNCTION update_ipomaster_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedDate = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_ipomaster_update
BEFORE UPDATE ON IpoMaster
FOR EACH ROW
EXECUTE FUNCTION update_ipomaster_timestamp();