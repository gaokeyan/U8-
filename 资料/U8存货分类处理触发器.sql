-- =============================================
-- Author:高科研
-- Create date: 2015-8-9
-- Description:	存货分类信息处理
-- =============================================
CREATE TRIGGER [dbo].[DEF1_Insert]
   ON  [dbo].[IA_Subsidiary]
   AFTER INSERT
AS 
BEGIN
	Declare @AutoID int 
	Declare @cInvCode varchar(500) 
    Declare @cDefine1 varchar(500)
    Declare @cName varchar(500)
    
    select @AutoID=AutoID,@cInvCode=cInvCode,@cDefine1=cDefine1 From inserted
    IF @cDefine1 IS NULL
    BEGIN
		set @cName=(SELECT (SELECT cInvCName FROM InventoryClass B WHERE B.cInvCCode=LEFT(A.cInvCCode,4)) FROM Inventory A WHERE A.cInvCode=@cInvCode)
		update IA_Subsidiary set cDefine1=@cName where AutoID=@AutoID
    END
    else
    BEGIN
		if LEN(@cDefine1)=0
		begin
			set @cName=(SELECT (SELECT cInvCName FROM InventoryClass B WHERE B.cInvCCode=LEFT(A.cInvCCode,4)) FROM Inventory A WHERE A.cInvCode=@cInvCode)
			update IA_Subsidiary set cDefine1=@cName where AutoID=@AutoID
		end
    END
END