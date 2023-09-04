aws gamecast create-application \
--description "new-demo-3" \
--executable-path  "u22.exe" \
--application-source-uri "s3://kiban-gamecast-us/demo" \
--runtime-environment "Type=PROTON,Version=20230428" \
--save-key "new-demo-save-key" \
--save-configuration "FileLocations=%userprofile%/AppData/LocalLow/DefaultCompany/u22" \
--profile gamecast2 \
--region us-east-2




aws gamecast create-stream-group \
--description "new demo" \
--default-application-identifier "id" \
--stream-class "gen4a_high" \
--profile gamecast2 \
--region us-east-2
