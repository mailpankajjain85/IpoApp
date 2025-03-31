CREATE TABLE UserRole (
    RoleName VARCHAR(20) PRIMARY KEY CHECK (RoleName IN ('SUPERADMIN', 'ADMIN', 'STAFF', 'CLIENT')),
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert predefined roles
INSERT INTO UserRole (RoleName) VALUES 
('SUPERADMIN'),
('ADMIN'),
('STAFF'),
('CLIENT');

CREATE TABLE ClientMaster (
    ClientID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrgShortCode VARCHAR(50) NOT NULL REFERENCES TenantOrg(OrgShortCode),
    ClientShortCode VARCHAR(20) NOT NULL,  -- Business identifier like CLI001
    FullName VARCHAR(255) NOT NULL,
    Email VARCHAR(255),
    Mobile VARCHAR(20) NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uk_client_org_code UNIQUE (OrgShortCode, ClientShortCode)
);

-- Indexes
CREATE INDEX idx_client_org ON ClientMaster(OrgShortCode);
CREATE INDEX idx_client_shortcode ON ClientMaster(ClientShortCode);

CREATE TABLE UserMaster (
    UserID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrgShortCode VARCHAR(50) NOT NULL REFERENCES TenantOrg(OrgShortCode),
    ClientID UUID UNIQUE NULL REFERENCES ClientMaster(ClientID),
    FullName VARCHAR(255) NOT NULL,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,  -- Same as ClientMaster email or different
    Role VARCHAR(20) NOT NULL REFERENCES UserRole(RoleName),
    Phone VARCHAR(20),
    IsClient BOOLEAN DEFAULT TRUE,  -- Explicit client role flag
    LastLogin TIMESTAMP WITH TIME ZONE,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Indexes
CREATE INDEX idx_user_client ON UserMaster(ClientID);
CREATE INDEX idx_user_email ON UserMaster(Email);


