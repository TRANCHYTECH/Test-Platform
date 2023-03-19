 dapr run --app-id proctor-manager --app-port 6300 --dapr-http-port 3602 --dapr-grpc-port 3603 dotnet run -urls "http://localhost:6300"

  dapr run --app-id test-runnwe --app-port 6200 --dapr-http-port 3604 --dapr-grpc-port 3605 dotnet run -urls "http://localhost:6200"


dapr invoke --app-id proctor-manager --method Exam/Content --verb GET