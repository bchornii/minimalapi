### Build docker image
~~~ps1
docker build -t miniapis:dev -f .\MinApiOnNet6\Dockerfile .

docker run --rm -it -p 8080:80 -e "Social__Google__ClientId=id" -e "Social__Google__ClientSecret=secret" -e "ASPNETCORE_URLS=http://+:80" minimalapi:dev

docker run --rm -it -p 8080:80 -e DOTNET_gcConcurrent=1 -e COMPlus_gcConcurrent=1 -e DOTNET_gcServer=0 -e DOTNET_GCHeapHardLimit=0xC800000 minimalapi:dev
docker run --rm -it -p 8080:80 -m 1024m --cpus=1 minimalapi:dev
~~~

### Start Bombarding (from local machine)
~~~ps1
bombardier.exe -c 120 --rate 100 -l -d 120s http://localhost:8080/blocking-threads

bombardier.exe -c 120 --rate 100 -l -d 120s http://localhost:8080/high-cpu

bombardier.exe -c 120 --rate 100 -l -d 120s http://localhost:8080/memory-leak
~~~

### Run container
#### Note: container will utilize all available resources, thus on multi core VM will result into Server GC
~~~ps1
docker run --rm -it -p 8080:80 minimalapi:dev
~~~

### Start Profiling
~~~ps1
docker exec -it <container-id> sh

./dotnet-counters monitor --process-id 1 --refresh-interval 3 --counters System.Runtime,Microsoft.AspNetCore.Hosting
~~~

### Retrieve results
~~~ps1
docker cp <container-id>:<path-to-the-dump> .
~~~

### Run k8s: deployment + node service
#### Note: based on a POD resource limits results in either Workstation or Server GC
~~~ps1
kubectl apply -f .\pod-and-node-svc.yml

kubectl delete -f .\pod-and-node-svc.yml
~~~