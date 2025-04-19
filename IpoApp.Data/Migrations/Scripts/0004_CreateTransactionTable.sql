CREATE TABLE TransactionMaster (
    TransactionID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrgShortCode VARCHAR(50) NOT NULL REFERENCES TenantOrg(OrgShortCode),
    ClientShortCode VARCHAR(20) NOT NULL,
    IPOId UUID NOT NULL REFERENCES IpoMaster(ID),
    SaudaType VARCHAR(20) NOT NULL REFERENCES SaudaType(TypeName),
    TransactionType VARCHAR(4) NOT NULL CHECK (TransactionType IN ('BUY', 'SELL')),
    AppType VARCHAR(10) NOT NULL REFERENCES IpoAppType(AppType),
    Quantity INTEGER NOT NULL CHECK (Quantity > 0),
    Price INTEGER NOT NULL,
    
    CreatedBy VARCHAR(50) NOT NULL REFERENCES UserMaster(Username),
    ModifiedBy VARCHAR(50) REFERENCES UserMaster(Username),
    CreatedDate TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    -- Foreign key to ClientMaster via composite reference
    CONSTRAINT fk_transaction_client FOREIGN KEY (OrgShortCode, ClientShortCode) 
        REFERENCES ClientMaster(OrgShortCode, ClientShortCode)
    
);

-- Indexes for performance
CREATE INDEX idx_transaction_org ON TransactionMaster(OrgShortCode);
CREATE INDEX idx_transaction_client ON TransactionMaster(ClientShortCode);
CREATE INDEX idx_transaction_ipo ON TransactionMaster(IPOId);
CREATE INDEX idx_transaction_dates ON TransactionMaster(CreatedDate, ModifiedDate);

CREATE OR REPLACE FUNCTION update_transaction_modified()
RETURNS TRIGGER AS $$
BEGIN
    NEW.ModifiedDate = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_transaction_modified
BEFORE UPDATE ON TransactionMaster
FOR EACH ROW
EXECUTE FUNCTION update_transaction_modified();