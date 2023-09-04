WORK_ROOT=$(pwd)
LOGDIR="${WORK_ROOT}/build.log"
SRCROOT="${WORK_ROOT}"

/Applications/Unity/Hub/Editor/2022.1.0f1/Unity.app/Contents/MacOS/Unity \
-batchmode \
-nographics \
-quit \
-projectPath "${SRCROOT}" \
-logFile "${LOGDIR}" \
-executeMethod App.BuildT.Build