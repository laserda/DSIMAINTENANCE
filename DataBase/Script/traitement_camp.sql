


declare @i int  = 0



if (OBJECT_ID('traitement_camp')) is null
begin

	DECLARE @datetime_folder nvarchar(20)
	DECLARE @path nvarchar(300)
	DECLARE @Save_data nvarchar(300)

	set @datetime_folder =replace(replace(replace(CONVERT(varchar, getdate(), 20),'-','') ,' ','_'),':','')
	set @path = 'C:\SAV\bases_before_traitement_camp' 

	EXECUTE master.dbo.xp_create_subdir   @path

	set @Save_data = @path + '\BDD_DSI_'+ @datetime_folder + '.bak'

	BACKUP DATABASE BDD_DSI TO  DISK =  @Save_data WITH NOFORMAT, NOINIT,  NAME = N'BDD_DSI-Complète Base de données Sauvegarde', SKIP, NOREWIND, NOUNLOAD,  STATS = 10


	create table traitement_camp(effectue bit default 0)
	insert into traitement_camp(effectue) values(1)

end
else
begin
	return	
end

if (OBJECT_ID('tempdb..#tmp_table')) is not null  

if (OBJECT_ID('tempdb..#tmp_table')) is not null  begin drop table  #tmp_table end

	SELECT 
		OBJECT_NAME([sm].[object_id])+'()' AS [ObjectName],
		[sm].[definition] AS [ObjectDefinition],
		sm.object_id,
		ROW_NUMBER() OVER (PARTITION BY 1  ORDER BY sm.object_id) as id
		into #tmp_table
	FROM sys.sql_modules sm INNER JOIN sys.objects o ON sm.object_id=o.object_id
	WHERE  
	(OBJECT_NAME([sm].[object_id]) LIKE '%WPLANTFOU%' or OBJECT_NAME([sm].[object_id]) LIKE '%WDEPARTEM%' or OBJECT_NAME([sm].[object_id]) LIKE '%WSOUSPREF%' 
	or OBJECT_NAME([sm].[object_id]) LIKE '%wregion%' or OBJECT_NAME([sm].[object_id]) LIKE '%WORIG%' or OBJECT_NAME([sm].[object_id]) LIKE '%WGPS%' 
	or OBJECT_NAME([sm].[object_id]) LIKE '%WEPOUSE%' or OBJECT_NAME([sm].[object_id]) LIKE '%WENFANT%' or OBJECT_NAME([sm].[object_id]) LIKE '%Wenclave%'
	) and 
	is_ms_shipped = 0 AND type_desc = 'SQL_INLINE_TABLE_VALUED_FUNCTION' 
	and OBJECT_NAME([sm].[object_id]) not in('wdepartement_travail','WENFANT_PERSO')



	if (OBJECT_ID('tempdb..#tmp_site')) is not null  begin drop table  #tmp_site end
	select cod_sit,ROW_NUMBER() OVER (PARTITION BY 1  ORDER BY recno) as id into #tmp_site from site


	
	declare @ObjectName nvarchar(30)
	declare @ObjectNameTable nvarchar(30)
	declare @numero nvarchar(30)
	declare @cod_sit nvarchar(30)
	declare @nocamp nvarchar(30),@j int = 0,@h int = 1
	
	declare @SQL_requete nvarchar(max)
	declare @list_des_champs nvarchar(max)

	if (OBJECT_ID('tempdb..#liste_des_table_column')) is not null  begin drop table  #liste_des_table_column end

	select TABLE_NAME,COLUMN_NAME into #liste_des_table_column from INFORMATION_SCHEMA.COLUMNS where  COLUMN_NAME not in('recno','diagram_id','timestamp_column','nocamp') order by TABLE_NAME,COLUMN_NAME



	

	while @h<=(select count(*) from #tmp_table)
	BEGIN

		set @ObjectName = (select ObjectName from #tmp_table where id = @h)
		
		set @numero = (select ltrim(rtrim(numero)) from WALIAS() where alias = @ObjectName)
		set @ObjectNameTable = (select new_alias from WALIAS() where alias = @ObjectName)
		set @list_des_champs=''
		SELECT @list_des_champs = COALESCE(@list_des_champs + ', ', '') + convert(varchar(30),COLUMN_NAME) 
		from #liste_des_table_column where TABLE_NAME=@numero 

		set @list_des_champs = SUBSTRING(@list_des_champs,2,len(@list_des_champs))

		if (OBJECT_ID('tempdb..##tmp_SAVE_value1')) is not null  begin drop table  ##tmp_SAVE_value1 end

		Set @SQL_requete='select * into ##tmp_SAVE_value1 from '+ @numero 	
		Exec (@SQL_requete)


		Set @SQL_requete='TRUNCATE TABLE '+ @numero 	
		Exec (@SQL_requete)

		

		SET @i = 1
		if CHARINDEX('@cod_sit',@ObjectNameTable) = 0
		BEGIN

			if (OBJECT_ID('tempdb..#tmp_camp1')) is not null  begin drop table  #tmp_camp1 end
			if (OBJECT_ID('tempdb..#tmp_camp_save1')) is not null  begin drop table  #tmp_camp_save1 end
			SELECT distinct ltrim(rtrim(nocamp))  nocamp into #tmp_camp1 FROM camp 
			SELECT nocamp,ROW_NUMBER() OVER (PARTITION BY 1  ORDER BY nocamp) as id  into #tmp_camp_save1 FROM #tmp_camp1 

			SET @j = 1
			while @j<=(select count(*) from #tmp_camp_save1)
			BEGIN

				set @nocamp = (select nocamp from #tmp_camp_save1 where id = @j)
				
				Set @SQL_requete='INSERT INTO ' + @numero + '('+@list_des_champs+',nocamp)' +' SELECT '+ @list_des_champs +',''' + @nocamp + '''  FROM ##tmp_SAVE_value1'		
				Exec (@SQL_requete)
				

				set @j = @j + 1
			END

		END
		ELSE
		BEGIN

			while @i<=(select count(*) from #tmp_site)
			BEGIN

				set @cod_sit = (select cod_sit from #tmp_site where id = @i)

				if (OBJECT_ID('tempdb..#tmp_camp2')) is not null  begin drop table  #tmp_camp2 end
				SELECT nocamp,ROW_NUMBER() OVER (PARTITION BY 1  ORDER BY recno) as id  into #tmp_camp2 FROM camp where   cod_sit = @cod_sit

				SET @j = 1
				while @j<=(select count(*) from #tmp_camp2)
				BEGIN

					set @nocamp = (select nocamp from #tmp_camp2 where id = @j)
					Set @SQL_requete='INSERT INTO ' + @numero + '('+@list_des_champs+',nocamp)' +' SELECT '+ @list_des_champs +',' + @nocamp + '  FROM ##tmp_SAVE_value1'		
					Exec (@SQL_requete)

					set @j = @j + 1
				END

				set @i = @i + 1
			END

		END

		set @h = @h + 1
	END