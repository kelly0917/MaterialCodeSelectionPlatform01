USE [MaterialCodeSelectionPlatform]
GO
/****** Object:  StoredProcedure [dbo].[SP_SysCommodityCode]    Script Date: 2020/2/5 0:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_SysCommodityCode]
	-- Add the parameters for the stored procedure here
	@UserId varchar(36),
	@CatalogName VARCHAR(50) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	 SET NOCOUNT ON;


	  
	  --INTO #tempPropertyValue
	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS CN_ShortDesc,b.INSTANCE_NO AS 材料编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempCN_ShortDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.CN_COMM_DESC_SHORT = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

	  SELECT DISTINCT a.ENTITY_PROPERTY_ID AS EN_ShortDesc,b.INSTANCE_NO AS 材料编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempEN_ShortDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.EN_COMM_DESC_SHORT = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

	   SELECT DISTINCT a.ENTITY_PROPERTY_ID AS RU_ShortDesc,b.INSTANCE_NO AS 材料编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempRU_ShortDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.RU_COMM_DESC_SHORT = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

	   SELECT DISTINCT a.ENTITY_PROPERTY_ID AS CN_LongDesc,b.INSTANCE_NO AS 材料编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempCN_LongDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.CN_COMM_DESC_LONG = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

	   SELECT DISTINCT a.ENTITY_PROPERTY_ID AS EN_LongDesc,b.INSTANCE_NO AS 材料编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempEN_LongDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.EN_COMM_DESC_LONG = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO

	   SELECT DISTINCT a.ENTITY_PROPERTY_ID AS RU_LongDesc,b.INSTANCE_NO AS 材料编码Id,b.PROPERTY_VALUE AS 属性值 INTO #tempRU_LongDesc  FROM  [dbo].[Temp_Property] a
	  INNER JOIN [dbo].Temp_PropertyValue b ON a.ENTITY_PROPERTY_NO = b.[ENTITY_PROPERTY_NO]
	  INNER JOIN dbo.Catalog c ON a.CATALOG_NO = c.Code AND c.RU_COMM_DESC_LONG = a.ENTITY_PROPERTY_ID
	  --WHERE a.CATALOG_NO = @CatalogNO


	SELECT NEWID() AS Id ,* INTO #tempCCData FROM (
	  SELECT  c.Id AS ComponentTypeId ,a.COMMODITY_ID AS Code,d.属性值 AS CN_ShortDesc,e.属性值 EN_ShortDesc,f.属性值 RU_ShortDesc,g.属性值 CN_LongDesc,h.属性值 EN_LongDesc,i.属性值 RU_LongDesc,0 AS Hits,0 Flag
	  ,0 Status,@UserId CreateUserId,GETDATE() CreateTime ,@UserId LastModifyUserId,GETDATE() LastModifyTime  FROM [dbo].[Temp_CommodityCode] a
	  INNER JOIN Temp_ComponentType b ON a.COMMODITY_CLASS_NO = b.CLASS_NO
	  INNER JOIN ComponentType c ON c.Code = b.CLASS_ID
	  LEFT JOIN #tempCN_ShortDesc d ON a.COMMODITY_NO = d.材料编码Id
	  LEFT JOIN #tempEN_ShortDesc e ON a.COMMODITY_NO = e.材料编码Id
      LEFT JOIN #tempRU_ShortDesc f ON a.COMMODITY_NO = f.材料编码Id
      LEFT JOIN #tempCN_LongDesc g ON a.COMMODITY_NO = g.材料编码Id
      LEFT JOIN #tempEN_LongDesc h ON a.COMMODITY_NO = h.材料编码Id
	  LEFT JOIN #tempRU_LongDesc i ON a.COMMODITY_NO = i.材料编码Id
	  ) a
	  ---新增 编码库
	  DECLARE @CCAddCount INT;

	  SELECT @CCAddCount = COUNT(1) FROM #tempCCData  a
	  LEFT JOIN [CommodityCode] b ON a.ComponentTypeId = b.ComponentTypeId AND a.Code = b.Code
	  WHERE b.Id IS NULL
	  	 PRINT 1
	  INSERT INTO [dbo].[CommodityCode]([Id]
      ,[ComponentTypeId]
      ,[Code]
      ,[CN_ShortDesc]
      ,[EN_ShortDesc]
      ,[RU_ShortDesc]
      ,[CN_LongDesc]
      ,[EN_LongDesc]
      ,[RU_LongDesc]
      ,[Hits]
      ,[Flag]
      ,[Status]
      ,[CreateUserId]
      ,[CreateTime]
      ,[LastModifyUserId]
      ,[LastModifyTime])
	  SELECT a.* FROM #tempCCData a
	  LEFT JOIN [CommodityCode] b ON a.ComponentTypeId = b.ComponentTypeId AND a.Code = b.Code
	  WHERE b.Id IS NULL
	  
	

	  DECLARE @CCDeleteCount INT;

	  ---删除
	  SELECT a.Id INTO #tempDeleteIds  FROM [CommodityCode] a
	  INNER JOIN ComponentType b ON a.ComponentTypeId = b.Id
	  INNER JOIN dbo.Catalog c ON b.CatalogId = c.Id
	  LEFT JOIN #tempCCData d ON d.code = a.Code
	  WHERE c.Name =@CatalogName AND d.Code IS NULL

	  SELECT @CCDeleteCount =COUNT(1) FROM #tempDeleteIds


	  DELETE FROM [CommodityCode] WHERE Id IN (SELECT Id FROM #tempDeleteIds)

	  DROP TABLE #tempCCData
	  DROP TABLE #tempCN_ShortDesc
	  DROP TABLE #tempCN_LongDesc

	  DROP TABLE #tempEN_LongDesc
	  DROP TABLE #tempEN_ShortDesc
	  DROP TABLE #tempRU_LongDesc
	  DROP TABLE #tempRU_ShortDesc

	  DROP TABLE #tempDeleteIds




	  --同步材料编码属性 

	 PRINT 2


	  SELECT NEWID() AS Id,c.ComponentTypeId,c.Id AS CommodityCodeId,a.SEQ_NO [SeqNo],a.descr [AttributeName],a.VALUE_TEXT [AttributeValue],0 [Flag],0 [Status]
	  ,@UserId [CreateUserId],GETDATE() [CreateTime],@UserId [LastModifyUserId],GETDATE() [LastModifyTime] INTO #tempPPData FROM dbo.Temp_CCPropertyValue a
	  INNER JOIN dbo.Temp_CommodityCode b ON a.instance_no = b.COMMODITY_NO
	  INNER JOIN dbo.CommodityCode c ON b.COMMODITY_ID = c.Code
	  
	   INSERT INTO CommodityCodeAttribute([Id]
      ,[ComponentTypeId]
      ,[CommodityCodeId]
      ,[SeqNo]
      ,[AttributeName]
      ,[AttributeValue]
      ,[Flag]
      ,[Status]
      ,[CreateUserId]
      ,[CreateTime]
      ,[LastModifyUserId]
      ,[LastModifyTime])
	  SELECT a.* FROM #tempPPData a
	  LEFT JOIN CommodityCodeAttribute b ON a.AttributeName = b.AttributeName AND a.AttributeValue = b.AttributeValue AND a.CommodityCodeId = b.CommodityCodeId
	  WHERE b.Id IS NULL



	  SELECT a.Id INTO #tempDeleteId1s FROM CommodityCodeAttribute a
	  INNER JOIN dbo.ComponentType c ON a.ComponentTypeId = c.Id
	  INNER JOIN dbo.Catalog d ON c.CatalogId = d.Id
	  LEFT JOIN #tempPPData b ON a.AttributeName = b.AttributeName AND a.AttributeValue = b.AttributeValue AND a.CommodityCodeId = b.CommodityCodeId

	  WHERE b.Id IS NULL AND d.Name =@CatalogName

	  DELETE a FROM CommodityCodeAttribute a 
	  INNER JOIN #tempDeleteId1s b ON a.Id = b.Id

	  DROP TABLE #tempPPData
	  DROP TABLE #tempDeleteId1s


END
