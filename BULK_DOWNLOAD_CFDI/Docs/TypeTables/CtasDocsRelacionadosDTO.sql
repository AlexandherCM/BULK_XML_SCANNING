CREATE TYPE dbo.CtasDocsRelacionadosDTO AS TABLE
(
    ID NVARCHAR(36),
    PagoID NVARCHAR(36),
    FacturaPPD_ID NVARCHAR(36),
    FormaPago NVARCHAR(2),
    NumParcialidad INT,
    ImpSaldoInsoluto DECIMAL(18, 2),
    ImpSaldoAnt DECIMAL(18, 2),
    ImpPagado DECIMAL(18, 2),
    IVA_Retenido DECIMAL(18,2),
    IVA_Trasladado DECIMAL(18,2),
    ISR_Retenido DECIMAL(18,2),
    ISR_Trasladado DECIMAL(18,2),
    IEPS_Retenido DECIMAL(18,2),
    IEPS_Trasladado DECIMAL(18,2)
)