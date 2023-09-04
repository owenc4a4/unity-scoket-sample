aws gamecast create-application \
--description "new-deom" \
--executable-path  "u22.exe" \
--application-source-uri "s3://kiban-gamecast-us/demo" \
--save-key "new-demo-save-key" \
--save-configuration "FileLocations=%userprofile%/AppData/LocalLow/DefaultCompany/u22"