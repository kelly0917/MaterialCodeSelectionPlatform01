USE [MaterialCodeSelectionPlatform]
GO
/****** Object:  StoredProcedure [dbo].[SP_SysComponentType]    Script Date: 2020/11/19 20:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_SysComponentType]
	@UserId varchar(36),
	@CatalogName VARCHAR(50)  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;

	--- 新增的
	declare @CatalogId varchar(36);

	select @CatalogId = Id from Catalog where Name =@CatalogName;
	
	SELECT newID() as id,* into #tempClass FROM ( 
	SELECT @CatalogId as CatalogId,@CatalogName as CatalogName,(case when PARENT_CLASS_NO is null then '00000000-0000-0000-0000-000000000000' else cast(PARENT_CLASS_NO as varchar(36)) end) as p,CLASS_ID,[DESCR],cast( CAN_INSTANTIATE as int) as CAN_INSTANTIATE,
	 UNIT_ID,DRAW_DISCIPLINE_NO,0 as dd,0 as s,@UserId as u1,getdate() as a,@UserId as u2,GETDATE() as b,CLASS_NO AS OrgId  from Temp_ComponentType where CATALOG_ID= @CatalogName and 
	CLASS_NO
	not in 
	(select a.OrgId from ComponentType a inner join Catalog b on a.CatalogId = b.Id where b.Name = @CatalogName)
	) a


	insert into ComponentType([Id]
      ,[CatalogId]
      ,[CatalogName]
      ,[ParentId]
      ,[Code]
      ,[Desc]
      ,[CanInstantiate]
      ,[Unit]
      ,[Discipline]
      ,[Flag]
      ,[Status]
      ,[CreateUserId]
      ,[CreateTime]
      ,[LastModifyUserId]
      ,[LastModifyTime],OrgId)
	  select * from #tempClass

	  update c set c.ParentId = p.Id from ComponentType c
	  INNER JOIN dbo.Temp_ComponentType a ON c.ParentId  = CAST( a.CLASS_NO AS VARCHAR(50)) AND a.CATALOG_ID=@CatalogName AND c.CatalogName =@CatalogName
	  inner join ComponentType p on a.CLASS_NO = p.OrgId


	--修改物资类型的描述
	SELECT newID() as id,* into #tempClass1 FROM ( 
	SELECT @CatalogId as CatalogId,@CatalogName as CatalogName,(case when PARENT_CLASS_NO is null then '00000000-0000-0000-0000-000000000000' else cast(PARENT_CLASS_NO as varchar(36)) end) as p,CLASS_ID,CLASS_NO,[DESCR],cast( CAN_INSTANTIATE as int) as CAN_INSTANTIATE,
	 UNIT_ID,DRAW_DISCIPLINE_NO,0 as dd,0 as s,@UserId as u1,getdate() as a,@UserId as u2,GETDATE() as b  from Temp_ComponentType where CATALOG_ID= @CatalogName and 
	[CLASS_NO]
	 in 
	(select a.OrgId from ComponentType a inner join Catalog b on a.CatalogId = b.Id where b.Name = @CatalogName)
	) a


	UPDATE a SET a.[Desc] = b.DESCR,a.Unit = b.UNIT_ID,a.Discipline= b.DRAW_DISCIPLINE_NO FROM ComponentType a
	INNER JOIN #tempClass1 b ON a.OrgId = b.CLASS_NO AND a.CatalogId = b.CatalogId AND a.CatalogName = b.CatalogName


	---删除的

	select a.Id into #tempDeleteIds from ComponentType a inner join Catalog b on a.CatalogId = b.Id where b.Name =@CatalogName and a.OrgId not in (
		select CLASS_NO from  Temp_ComponentType
	)
	
	declare @deleteCount int 
	select @deleteCount = count(1) from #tempDeleteIds

	delete from ComponentType where Id in (select Id from #tempDeleteIds)

	DROP TABLE #tempClass
	DROP TABLE #tempDeleteIds
	DROP TABLE #tempClass1
	

END
