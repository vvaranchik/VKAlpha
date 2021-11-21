namespace Alpha.SharedLibrary.Interfaces
{

    public interface IMethod<ServiceInstance>
    {
        ServiceInstance service { get; set; }
        HttpClient client { get; set; }

        public bool IsValid(/*bool renew*/); // Checks is access token stored in service and is still valid
    }
}
