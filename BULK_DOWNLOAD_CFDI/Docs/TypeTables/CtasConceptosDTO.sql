CREATE TYPE dbo.CtasConceptosDTO AS TABLE
(
    ID NVARCHAR(36),
    FacturaID NVARCHAR(36),
    ClaveProductoServicio NVARCHAR(8),
    ClaveUnidad NVARCHAR(5),
    Cantidad INT,
    Descripcion NVARCHAR(MAX),  -- ← Aquí el cambio
    ValorUnitario DECIMAL(18,2),
    Importe DECIMAL(18,2),
    ImporteFinal DECIMAL(18,2),
    IVA_Retenido DECIMAL(18,2),
    IVA_Trasladado DECIMAL(18,2),
    ISR_Retenido DECIMAL(18,2),
    ISR_Trasladado DECIMAL(18,2),
    IEPS_Retenido DECIMAL(18,2),
    IEPS_Trasladado DECIMAL(18,2)
);
