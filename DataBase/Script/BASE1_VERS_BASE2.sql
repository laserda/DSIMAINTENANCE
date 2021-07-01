
if OBJECT_ID('tempdb..#liste_des_table') is not null DROP TABLE #liste_des_table 
if OBJECT_ID('tempdb..#liste_des_table_id') is not null DROP TABLE #liste_des_table_id 

declare @table_name varchar(40)
declare @BaseDeDonneesDeDestination varchar(40)
declare @BaseDeDonneesSource varchar(40)

declare @SQL_requete nvarchar(max)
declare @nb_enregistrement numeric
declare @nb_enregistrement2 numeric
declare @list_des_champs nvarchar(max)



Set @BaseDeDonneesSource='DIRECTAPPDATALIASSE_REF'
set @BaseDeDonneesDeDestination='DIRECTAPPDATALIASSE_vide'
 

USE DIRECTAPPDATALIASSE_REF

SELECT * into  #liste_des_table from dbo.sysobjects  
WHERE xtype= 'U' and  SUBSTRING(name,1,7)<>'MSmerge' and SUBSTRING(name,1,4)<>'vtmp' and SUBSTRING(name,1,8)<>'sysmerge' order by NAME


select  ROW_NUMBER() OVER(ORDER BY name ASC) as id,name into #liste_des_table_id  from #liste_des_table where name not in('Translations','Cultures','sp0000094','b00002280','p00000603','p00020200') order by name
DECLARE @cnt INT = 1;
set @nb_enregistrement=(select count(*) from #liste_des_table_id)
declare @compteur int
set @compteur = 0
declare @compteur2 int

if OBJECT_ID('tempdb..#liste_des_table_column') is not null DROP TABLE #liste_des_table_column 
if OBJECT_ID('tempdb..#liste_des_table_column_2') is not null DROP TABLE #liste_des_table_column_2 

 
WHILE @cnt < @nb_enregistrement-1
BEGIN
	set @table_name=(SELECT  name  FROM #liste_des_table_id WHERE id=@cnt)




	set @list_des_champs=''	
	if OBJECT_ID('tempdb..#liste_des_table_column') is not null DROP TABLE #liste_des_table_column 
	if OBJECT_ID('tempdb..#liste_des_table_column_2') is not null DROP TABLE #liste_des_table_column_2 
			 
	select distinct * into #liste_des_table_column  from INFORMATION_SCHEMA.COLUMNS 
	where TABLE_CATALOG = @BaseDeDonneesSource and TABLE_NAME=@table_name and data_type <> 'uniqueidentifier' order by TABLE_NAME 
	---select COLUMN_NAME from #liste_des_table_column
	
	select  @list_des_champs = COALESCE(@list_des_champs + ',', '') + '' + 
	convert(nvarchar(max),COLUMN_NAME) 
	FROM #liste_des_table_column where  COLUMN_NAME not in('','recno','timestamp_column')
	
	set @list_des_champs = SUBSTRING(@list_des_champs,2,len(@list_des_champs))

	--PRINT @list_des_champs
	
	
	
	Set @SQL_requete='TRUNCATE TABLE '+ @BaseDeDonneesDeDestination +'.dbo.'+ @table_name 
	Exec (@SQL_requete)
	
	Set @SQL_requete='INSERT INTO '+ @BaseDeDonneesDeDestination +'.dbo.'+ @table_name + '('+@list_des_champs+')' +' SELECT '+ @list_des_champs +'  FROM ' +@BaseDeDonneesSource + '.dbo.' + @table_name 
	begin try
		Exec (@SQL_requete)
	end try
	begin catch
		select * from  #liste_des_table_column 
		PRINT @list_des_champs
		PRINT @table_name
		PRINT @SQL_requete
	end catch

	
			
	SET @cnt = @cnt + 1

END 
 