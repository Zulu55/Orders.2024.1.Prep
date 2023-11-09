namespace Orders.Frontend.Repositories
{
    public interface IRepository
    {
        Task<HttpResponseWrapper<T>> GetAsync<T>(string url);

        Task<HttpResponseWrapper<object>> GetAsync(string url);

        Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model);

        Task<HttpResponseWrapper<TResponse>> PostAsync<T, TResponse>(string url, T model);

        Task<HttpResponseWrapper<object>> DeleteAsync(string url);

        Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model);

        Task<HttpResponseWrapper<TResponse>> PutAsync<T, TResponse>(string url, T model);
    }
}