
CREATE TYPE dbo.CtasPagosFacturaDTO AS TABLE
(
    ID NVARCHAR(36),
    FacturaPagoID NVARCHAR(36),
    FormaPago NVARCHAR(2),
    NoPago INT,
    NumOperacion NVARCHAR(100),
    MontoTotal DECIMAL(18, 2),
    IVA_Retenido DECIMAL(18,2),
    IVA_Trasladado DECIMAL(18,2),
    ISR_Retenido DECIMAL(18,2),
    ISR_Trasladado DECIMAL(18,2),
    IEPS_Retenido DECIMAL(18,2),
    IEPS_Trasladado DECIMAL(18,2)
)