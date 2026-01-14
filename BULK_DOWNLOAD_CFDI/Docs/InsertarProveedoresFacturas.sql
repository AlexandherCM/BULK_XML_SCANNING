INSERT INTO Proveedor (
    RFC,
    PersonaMoral,
    Nombre,
    RegimenFiscal,
    Estatus
)
SELECT
    f.RFC_Emisor AS RFC,
    CASE 
        WHEN LEN(f.RFC_Emisor) = 12 THEN 1  -- Persona Moral
        WHEN LEN(f.RFC_Emisor) = 13 THEN 0  -- Persona Física
        ELSE NULL
    END AS PersonaMoral,
    f.Nombre_Emisor AS Nombre,
    f.RegimenEmisor AS RegimenFiscal,
    1 AS Estatus
FROM CtasFacturas f
WHERE f.Emitida = 0
  AND f.RFC_Emisor IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 
      FROM Proveedor p 
      WHERE p.RFC = f.RFC_Emisor
  )
GROUP BY
    f.RFC_Emisor,
    f.Nombre_Emisor,
    f.RegimenEmisor;
