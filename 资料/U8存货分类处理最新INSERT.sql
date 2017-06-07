-- =============================================
-- Author:高科研
-- Create date: 2015-8-18
-- Description:	存货分类信息INSERT处理【已经存在时：CREATE更改为ALTER】
-- =============================================
CREATE TRIGGER [dbo].[DEF1_Insert]
   ON  [dbo].[IA_Subsidiary]
   AFTER INSERT
AS 
BEGIN
	Declare @cInvCode varchar(500) 
    Declare @cDefine1 varchar(500)    
    select @cInvCode=cInvCode,@cDefine1=cDefine1 From inserted
    IF @cInvCode IS NOT NULL
    BEGIN
		UPDATE  IA_Subsidiary SET cDefine1=BB.CName
		FROM IA_Subsidiary AA,
		(SELECT * FROM
		(
		select AutoID,cInvCode,
			(SELECT (SELECT cInvCName FROM InventoryClass B 
					 WHERE B.cInvCCode=LEFT(A.cInvCCode,4)) 
			 FROM Inventory A WHERE A.cInvCode=C.cInvCode) AS CName 
		from IA_Subsidiary C 
		where dCreateDate>=cast(CONVERT(varchar,GETDATE(),111) as datetime) 
		and (cInvCode is not null or len(cInvCode)<>0) 
		and (cDefine1 is null or len(cDefine1)=0)
		) D) BB
		WHERE AA.AutoID=BB.AutoID
    END
END

