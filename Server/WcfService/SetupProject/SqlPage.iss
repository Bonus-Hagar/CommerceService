[Code]

var
  SQLPage_lblSQLComment: TLabel;
  SQLPage_lblServer: TLabel;
  SQLPage_lblAuthType: TLabel;
  SQLPage_lblUser: TLabel;
  SQLPage_lblPassword: TLabel;
  SQLPage_lblDatabase: TLabel;
  SQLPage_chkSQLAuth: TRadioButton;
  SQLPage_chkWindowsAuth: TRadioButton;
  SQLPage_xCreateUser: TCheckBox; 
  SQLPage_ConnectButton : TButton;

  SQLPage_txtServer: TEdit;
  SQLPage_txtUsername: TEdit;
  SQLPage_txtPassword: TPasswordEdit;
  SQLPage_txtDBname: TEdit;

  SQLPage: TWizardPage;


// enable/disable child text boxes & functions when text has been entered into Server textbox. Makes no sense to populate child items unless a value exists for server.
procedure SQLServerOnChange(Sender: TObject);
begin   
  Log('SQLServerOnChange called');
  WizardForm.NextButton.Enabled := False;
  if (Length(SQLPage_txtDBname.Text) > 0) and (Length(SQLPage_txtServer.Text) > 0) then
  begin
    WizardForm.NextButton.Enabled := True;
    SQLPage_ConnectButton.Enabled := True;
  end;
                        
  if Length(SQLPage_txtServer.Text) > 0 then
  begin
    SQLPage_lblAuthType.Enabled := True;
    SQLPage_chkWindowsAuth.Enabled := True;
    SQLPage_chkSQLAuth.Enabled := True;
  end
  else
  begin
    SQLPage_lblAuthType.Enabled := False;
    SQLPage_chkWindowsAuth.Enabled := False;
    SQLPage_chkSQLAuth.Enabled := False;
  end
end;

// enable/disable user/pass text boxes depending on selected auth type. A user/pass is only required for SQL Auth
procedure SQLAuthOnChange(Sender: TObject);
begin
  Log('SQLAuthOnChange called');
  if SQLPage_chkWindowsAuth.Checked then
  begin
    SQLPage_lblUser.Enabled := False;
    SQLPage_lblPassword.Enabled := False;
    SQLPage_txtUsername.Enabled := False;
    SQLPage_txtPassword.Enabled := False;
	  SQLPage_xCreateUser.Checked := False;
  end
  else
  begin
    SQLPage_lblUser.Enabled := True;
    SQLPage_lblPassword.Enabled := True;
    SQLPage_txtUsername.Enabled := True;
    SQLPage_txtPassword.Enabled := True;
	  SQLPage_xCreateUser.Checked := True;
  end
end;

function SQLPageADOTestConnection(dbServer: string; userName: string; pwd: string; windowsAuth: Boolean): Variant;
var  
  ADOConnection: Variant;  
begin
  Log('SQLPageADOTestConnection called');

  try
    dbServer := Trim(dbServer);
    userName := Trim(userName);
    pwd := Trim(pwd);
 
    // create the ADO connection object
    ADOConnection := CreateOleObject('ADODB.Connection');
    // build a connection string; 
    ADOConnection.ConnectionString := 'Provider=SQLOLEDB;Data Source=' + dbServer + ';Application Name= My Execute SQL;';

    if windowsAuth then
      ADOConnection.ConnectionString := ADOConnection.ConnectionString + 'Integrated Security=SSPI;'         // Windows Auth
    else
      ADOConnection.ConnectionString := ADOConnection.ConnectionString + 'User Id=' + userName + ';Password=' + pwd + ';';

    Log('SQLPageADOTestConnection - Test:' + ADOConnection.ConnectionString);
    ADOConnection.Open;
    ADOConnection.Close;
    MsgBox('Success '#13#13 'Connected !', mbInformation, MB_OK);
  except
    MsgBox(GetExceptionMessage, mbError, MB_OK);
  end;

  // open the connection by the assigned ConnectionString
  Result := ADOConnection;
end;

procedure SQLPageTestConnectionClick(Sender: TObject);
var
  ADOConnection: Variant; 
begin
  Log('SQLPageTestConnectionClick called');
  ADOConnection := SQLPageADOTestConnection(SQLPage_txtServer.Text, SQLPage_txtUsername.Text, SQLPage_txtPassword.Text, SQLPage_chkWindowsAuth.Checked);
end;

procedure SQLCustomForm_Activate(Page: TWizardPage);
begin
  Log('SQLCustomForm_Activate called');
  WizardForm.NextButton.Enabled := False;
  if (Length(SQLPage_txtDBname.Text) > 0) and (Length(SQLPage_txtServer.Text) > 0)  then
  begin
    WizardForm.NextButton.Enabled := True;
    SQLPage_ConnectButton.Enabled := True;
  end;
end;

{ SQLCustomForm_CreatePage }
function SQLCustomForm_CreatePage(PreviousPageId: Integer): TWizardPage;
begin
  SQLPage := CreateCustomPage(
    PreviousPageId,
    'SQL Sever database for Commerce Service for LS Central',
    'Creates LS Commerce objects in a new or existing SQL database'
  );
 
  { lblServer }
  SQLPage_lblServer := TLabel.Create(SQLPage);
  with SQLPage_lblServer do
  begin
    Parent := SQLPage.Surface;
    Caption :=  'SQL Server name:';
    Left := ScaleX(24);
    Top := ScaleY(9);
    Width := ScaleX(115);
    Height := ScaleY(13);
    Enabled := True;
  end;
  { txtServer }
  SQLPage_txtServer := TEdit.Create(SQLPage);
  with SQLPage_txtServer do
  begin
    Parent := SQLPage.Surface;
    Left := ScaleX(152);
    Top := ScaleY(6);
    Width := ScaleX(225);
    Height := ScaleY(21);
    TabOrder := 1;
    Enabled := True;
    OnChange := @SQLServerOnChange;
    ShowHint := True;
    Hint := 'SQL Server name and instance where LS Commerce Database will be created on.';
  end;

  { lblDatabase }
  SQLPage_lblDatabase := TLabel.Create(SQLPage);
  with SQLPage_lblDatabase do
  begin
    Parent := SQLPage.Surface;
    Caption := 'SQL Database name:';
    Left := ScaleX(24);
    Top := ScaleY(33);
    Width := ScaleX(115);
    Height := ScaleY(13);
    Enabled := True;
  end;
  { txtDBname }
  SQLPage_txtDBname := TEdit.Create(SQLPage);
  with SQLPage_txtDBname do
  begin
    Parent := SQLPage.Surface;
    Left := ScaleX(152);
    Top := ScaleY(30);
    Width := ScaleX(225);
    Height := ScaleY(21);
    Enabled := True;
    OnChange := @SQLServerOnChange;
    TabOrder := 2;
    ShowHint := True;
    Hint := 'Database name for LS Commerce Database.';
  end;

  { lblSQLComment }
  SQLPage_lblSQLComment := TLabel.Create(SQLPage);
  with SQLPage_lblSQLComment do
  begin
    Parent := SQLPage.Surface;
    Caption := 'A new database is created if one does not exist';
    Left := ScaleX(24);
    Top := ScaleY(54);
    Width := ScaleX(250);
    Height := ScaleY(13);
  end;

  SQLPage_xCreateUser := TCheckBox.Create(SQLPage);
  with SQLPage_xCreateUser do
  begin
    Parent := SQLPage.Surface;
    Left := ScaleX(24);
    Top := ScaleY(70);
    Width := ScaleX(380);
    Height := ScaleY(21);
    Caption := 'Create and use CommerceUser to connect to Commerce Database';
    Checked := True;
    TabOrder := 3;
    ShowHint := True;
    Hint := 'Create CommerceUser on SQL Server and use it when connecting to the Database. If not checked, the SQL Credentials will be used to connect.';
  end;

  { lblAuthType }
  SQLPage_lblAuthType := TLabel.Create(SQLPage);
  with SQLPage_lblAuthType do
  begin
    Parent := SQLPage.Surface;
    Caption :=  'SQL credentials';
    Left := ScaleX(24);
    Top := ScaleY(96);
    Width := ScaleX(87);
    Height := ScaleY(13);
    Enabled := True;
  end;

  { chkWindowsAuth }
  SQLPage_chkWindowsAuth := TRadioButton.Create(SQLPage);
  with SQLPage_chkWindowsAuth do
  begin
    Parent := SQLPage.Surface;
    Caption := 'Use Windows Authentication';
    Left := ScaleX(32);
    Top := ScaleY(113);
    Width := ScaleX(177);
    Height := ScaleY(17);
    Checked := True;
    TabOrder := 4;
    TabStop := True;
    OnClick := @SQLAuthOnChange;
    Enabled := True;
  end;

  { chkSQLAuth }
  SQLPage_chkSQLAuth := TRadioButton.Create(SQLPage);
  with SQLPage_chkSQLAuth do
  begin
    Parent := SQLPage.Surface;
    Caption := 'Use SQL Server Authentication';
    Left := ScaleX(32);
    Top := ScaleY(133);
    Width := ScaleX(185);
    Height := ScaleY(17);
    Checked := False;
    TabOrder := 5;
    OnClick := @SQLAuthOnChange;
    Enabled := True;
  end;

  { lblUser }
  SQLPage_lblUser := TLabel.Create(SQLPage);
  with SQLPage_lblUser do
  begin
    Parent := SQLPage.Surface;
    Caption := 'User (sysadmin):' ;
    Left := ScaleX(60);
    Top := ScaleY(156);
    Width := ScaleX(85);
    Height := ScaleY(13);
    Enabled := False;
  end;
  { txtUsername }
  SQLPage_txtUsername := TEdit.Create(SQLPage);
  with SQLPage_txtUsername do
  begin
    Parent := SQLPage.Surface;
    Left := ScaleX(152);
    Top := ScaleY(153);
    Width := ScaleX(225);
    Height := ScaleY(21);
    Enabled := False;
    TabOrder := 6;
  end;

  { lblPassword }
  SQLPage_lblPassword := TLabel.Create(SQLPage);
  with SQLPage_lblPassword do
  begin
    Parent := SQLPage.Surface;
    Caption := 'Password:' ;
    Left := ScaleX(60);
    Top := ScaleY(174);
    Width := ScaleX(85);
    Height := ScaleY(13);
    Enabled := False;
  end;
  { txtPassword }
  SQLPage_txtPassword := TPasswordEdit.Create(SQLPage);
  with SQLPage_txtPassword do
  begin
    Parent := SQLPage.Surface;
    Left := ScaleX(152);
    Top := ScaleY(177);
    Width := ScaleX(225);
    Height := ScaleY(21);
    Enabled := False;
    TabOrder := 7;
  end;

  { SQLPage_ConnectButton }
  SQLPage_ConnectButton := TButton.Create(SQLPage);
  with SQLPage_ConnectButton do
  begin
    Parent := SQLPage.Surface;
    Left := ScaleX(152);
    Top := ScaleY(201);
    Width := ScaleX(121);
    Height := ScaleY(21);
    Enabled := False;
    TabOrder := 8;
    Caption := 'Test SQL Connection';
    ShowHint := True;
    Hint := 'Test SQL Connection to LS Commerce Database with provided log on credentials.';
    OnClick := @SQLPageTestConnectionClick;
  end;

  //does not work except from main form
  with SQLPage do
  begin
    OnActivate := @SQLCustomForm_Activate;
  end;

  Result := SQLPage;
end;
