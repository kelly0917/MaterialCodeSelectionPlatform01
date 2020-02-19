USE [MaterialCodeSelectionPlatform]
GO
/****** Object:  StoredProcedure [dbo].[SP_SysPartNumber]    Script Date: 2020/2/19 0:32:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_SysPartNumber]
	-- Add the parameters for the stored procedure here
	@UserId varchar(36),
	@CatalogName VARCHAR(50)  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	
	  --INTO #tempPropertyValue
	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS CN_ShortDesc,b.INSTANCE_NO AS 采购编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempCN_ShortDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.CN_PART_DESC_SHORT = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS EN_ShortDesc,b.INSTANCE_NO AS 采购编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempEN_ShortDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.EN_PART_DESC_SHORT = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

      SELECT DISTINCT a.ENTITY_PROPERTY_ID AS RU_ShortDesc,b.INSTANCE_NO AS 采购编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempRU_ShortDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.RU_PART_DESC_SHORT = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS CN_LongDesc,b.INSTANCE_NO AS 采购编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempCN_LongDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.CN_PART_DESC_LONG = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS EN_LongDesc,b.INSTANCE_NO AS 采购编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempEN_LongDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.EN_PART_DESC_LONG = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS RU_LongDesc,b.INSTANCE_NO AS 采购编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempRU_LongDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.RU_PART_DESC_LONG = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 


	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS CN_SizeDesc,b.INSTANCE_NO AS SizeRefId,b.PROPERTY_VALUE AS 属性值 INTO #tempCN_SizeDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.CN_SIZE_DESC = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS EN_SizeDesc,b.INSTANCE_NO AS SizeRefId,b.PROPERTY_VALUE AS 属性值 INTO #tempEN_SizeDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.EN_SIZE_DESC = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS RU_SizeDesc,b.INSTANCE_NO AS SizeRefId,b.PROPERTY_VALUE AS 属性值 INTO #tempRU_SizeDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.RU_SIZE_DESC = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO 

	  SELECT NEWID() AS Id ,* INTO #tempCGData FROM (
	  SELECT  DISTINCT a.PART_ID AS Code, n.ComponentTypeId,n.Id AS CommodityCodeId,d.属性值 AS CN_ShortDesc,e.属性值 EN_ShortDesc,f.属性值 RU_ShortDesc,g.属性值 CN_LongDesc,h.属性值 EN_LongDesc,i.属性值 RU_LongDesc,k.属性值 AS CN_SizeDesc
	  ,x.属性值 AS EN_SizeDesc,y.属性值 AS RU_SizeDesc,0 Flag
	  ,0 Status,@UserId CreateUserId,GETDATE() CreateTime ,@UserId LastModifyUserId,GETDATE() LastModifyTime   FROM [dbo].Temp_PartNumber a
	  INNER JOIN Temp_CommodityCode m ON a.COMMODITY_NO = m.COMMODITY_NO
	  INNER JOIN CommodityCode n ON m.COMMODITY_ID = n.Code
	  LEFT JOIN #tempCN_ShortDesc d ON a.PART_NO = d.采购编码Id
	  LEFT JOIN #tempEN_ShortDesc e ON a.PART_NO = e.采购编码Id
      LEFT JOIN #tempRU_ShortDesc f ON a.PART_NO = f.采购编码Id
      LEFT JOIN #tempCN_LongDesc g ON a.PART_NO = g.采购编码Id
      LEFT JOIN #tempEN_LongDesc h ON a.PART_NO = h.采购编码Id
	  LEFT JOIN #tempRU_LongDesc i ON a.PART_NO = i.采购编码Id
	  LEFT JOIN #tempCN_SizeDesc k ON a.SIZE_REF_NO = k.SizeRefId
	  LEFT JOIN #tempEN_SizeDesc x ON a.SIZE_REF_NO = x.SizeRefId
	  LEFT JOIN #tempRU_SizeDesc y ON a.SIZE_REF_NO = y.SizeRefId
	)a

	    ---新增 采购码
	  DECLARE @CGAddCount INT;
	  SELECT @CGAddCount = COUNT(1) FROM #tempCGData a
	  LEFT JOIN PartNumber b ON a.Code = b.Code AND a.ComponentTypeId = b.ComponentTypeId AND a.CommodityCodeId = b.CommodityCodeId
	  WHERE b.Id IS NULL
	  



	  INSERT INTO [dbo].PartNumber([Id]
      ,[Code]
      ,[ComponentTypeId]
      ,[CommodityCodeId]
      ,[CN_ShortDesc]
      ,[EN_ShortDesc]
      ,[RU_ShortDesc]
      ,[CN_LongDesc]
      ,[EN_LongDesc]
      ,[RU_LongDesc]
      ,[CN_SizeDesc]
      ,[EN_SizeDesc]
      ,[RU_SizeDesc]
      ,[Flag]
      ,[Status]
      ,[CreateUserId]
      ,[CreateTime]
      ,[LastModifyUserId]
      ,[LastModifyTime])
	  SELECT a.* FROM #tempCGData   a
	  LEFT JOIN PartNumber b ON a.Code = b.Code AND a.ComponentTypeId = b.ComponentTypeId AND a.CommodityCodeId = b.CommodityCodeId
	  WHERE b.Id IS NULL

	  --修改

	  UPDATE b SET b.CN_ShortDesc =a.CN_ShortDesc,b.EN_ShortDesc= a.EN_ShortDesc,b.RU_ShortDesc = a.RU_ShortDesc,b.CN_LongDesc=a.CN_LongDesc,b.EN_LongDesc=a.EN_LongDesc,b.CN_SizeDesc=a.CN_SizeDesc,b.EN_SizeDesc=a.EN_SizeDesc,b.RU_SizeDesc=a.RU_SizeDesc  FROM #tempCGData a
	  INNER JOIN PartNumber b ON a.Code = b.Code AND a.ComponentTypeId = b.ComponentTypeId AND a.CommodityCodeId = b.CommodityCodeId



	  DECLARE @CGDeleteCount INT;

	  ---删除
	  SELECT a.Id INTO #tempDeleteIds FROM PartNumber a
	  INNER JOIN ComponentType b ON a.ComponentTypeId = b.Id
	  INNER JOIN dbo.Catalog c ON b.CatalogId = c.Id
	  LEFT JOIN #tempCGData d ON a.code = d.Code
	  WHERE c.Name =@CatalogName AND d.Code IS NULL
	  SELECT @CGDeleteCount =COUNT(1) FROM #tempDeleteIds


	  DELETE FROM PartNumber WHERE Id IN (SELECT Id FROM #tempDeleteIds)

	  DROP TABLE #tempCGData
	  DROP TABLE #tempCN_ShortDesc
	  DROP TABLE #tempCN_LongDesc

	  DROP TABLE #tempEN_LongDesc
	  DROP TABLE #tempEN_ShortDesc
	  DROP TABLE #tempRU_LongDesc
	  DROP TABLE #tempRU_ShortDesc

	  DROP TABLE #tempDeleteIds

	  DROP TABLE #tempEN_SizeDesc
	  DROP TABLE #tempCN_SizeDesc
	  DROP TABLE #tempRU_SizeDesc



END
