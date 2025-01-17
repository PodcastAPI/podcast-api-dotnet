﻿using PodcastAPI.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodcastAPI
{
    public sealed class Client
    {
        public readonly string BASE_URL_TEST = "https://listen-api-test.listennotes.com/api/v2";
        public readonly string BASE_URL_PROD = "https://listen-api.listennotes.com/api/v2";
        public readonly IRestClient restClient;
        public readonly string userAgent;

        public Client(string apiKey = null)
        {
            var version = GetType().Assembly.GetName().Version.ToString();

            userAgent = $"podcasts-api-dotnet {version}";

            var prodMode = !string.IsNullOrWhiteSpace(apiKey);

            restClient = new RestClient(prodMode ? BASE_URL_PROD : BASE_URL_TEST);

            if (prodMode)
            {
                restClient.AddDefaultHeader("X-ListenAPI-Key", apiKey);
            }

            // default is 30 seconds
            restClient.Timeout = 30000;

            restClient.AddDefaultHeader("User-Agent", userAgent);
        }

        public async Task<ApiResponse> Search(IDictionary<string, string> parameters)
        {
            var url = "search";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> Typeahead(IDictionary<string, string> parameters)
        {
            var url = "typeahead";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> SpellCheck(IDictionary<string, string> parameters)
        {
            var url = "spellcheck";
            return await Get(url, parameters);
        }        

        public async Task<ApiResponse> FetchRelatedSearches(IDictionary<string, string> parameters)
        {
            var url = "related_searches";
            return await Get(url, parameters);
        }        

        public async Task<ApiResponse> FetchTrendingSearches(IDictionary<string, string> parameters)
        {
            var url = "trending_searches";
            return await Get(url, parameters);
        } 

        public async Task<ApiResponse> FetchBestPodcasts(IDictionary<string, string> parameters)
        {
            var url = "best_podcasts";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchPodcastGenres(IDictionary<string, string> parameters)
        {
            var url = "genres";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchPodcastRegions(IDictionary<string, string> parameters = null)
        {
            var url = "regions";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchPodcastLanguages(IDictionary<string, string> parameters = null)
        {
            var url = "languages";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchPodcastById(IDictionary<string, string> parameters)
        {
            var url = $"podcasts/{parameters["id"]}";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchEpisodeById(IDictionary<string, string> parameters)
        {
            var url = $"episodes/{parameters["id"]}";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchCuratedPodcastsListById(IDictionary<string, string> parameters)
        {
            var url = $"curated_podcasts/{parameters["id"]}";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchCuratedPodcastsLists(IDictionary<string, string> parameters)
        {
            var url = "curated_podcasts";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> JustListen(IDictionary<string, string> parameters = null)
        {
            var url = $"just_listen";

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchRecommendationsForPodcast(IDictionary<string, string> parameters)
        {
            var url = $"podcasts/{parameters["id"]}/recommendations";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchRecommendationsForEpisode(IDictionary<string, string> parameters)
        {
            var url = $"episodes/{parameters["id"]}/recommendations";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchPlaylistById(IDictionary<string, string> parameters)
        {
            var url = $"playlists/{parameters["id"]}";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchMyPlaylists(IDictionary<string, string> parameters)
        {
            var url = "playlists";
            return await Get(url, parameters);
        }

        public async Task<ApiResponse> SubmitPodcast(IDictionary<string, string> parameters)
        {
            var url = "podcasts/submit";
            return await Post(url, parameters);
        }

        public async Task<ApiResponse> BatchFetchPodcasts(IDictionary<string, string> parameters)
        {
            var url = "podcasts";
            return await Post(url, parameters);
        }

        public async Task<ApiResponse> BatchFetchEpisodes(IDictionary<string, string> parameters)
        {
            var url = "episodes";
            return await Post(url, parameters);
        }

        public async Task<ApiResponse> DeletePodcast(IDictionary<string, string> parameters)
        {
            var url = $"podcasts/{parameters["id"]}";
            parameters.Remove("id");

            return await Delete(url, parameters);
        }

        public async Task<ApiResponse> FetchAudienceForPodcast(IDictionary<string, string> parameters)
        {
            var url = $"podcasts/{parameters["id"]}/audience";
            parameters.Remove("id");

            return await Get(url, parameters);
        }

        public async Task<ApiResponse> FetchPodcastsByDomain(IDictionary<string, string> parameters)
        {
            var url = $"podcasts/domains/{parameters["domain_name"]}";
            parameters.Remove("domain_name");

            return await Get(url, parameters);
        }        

        private async Task<ApiResponse> Get(string url, IDictionary<string, string> parameters = null)
        {
            var request = new RestRequest(url, Method.GET);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddQueryParameter(parameter.Key, parameter.Value);
                }
            }

            var response = await restClient.ExecuteAsync(request);

            ProcessStatus((int)response.StatusCode);

            var result = new ApiResponse(response.Content, response);

            return result;
        }

        private async Task<ApiResponse> Post(string url, IDictionary<string, string> parameters)
        {
            var request = new RestRequest(url, Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value);
                }
            }

            var response = await restClient.ExecuteAsync(request);

            ProcessStatus((int)response.StatusCode);

            var result = new ApiResponse(response.Content, response);

            return result;
        }

        private async Task<ApiResponse> Delete(string url, IDictionary<string, string> parameters)
        {
            var request = new RestRequest(url, Method.DELETE);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddQueryParameter(parameter.Key, parameter.Value);
                }
            }

            var response = await restClient.ExecuteAsync(request);

            ProcessStatus((int)response.StatusCode);

            var result = new ApiResponse(response.Content, response);

            return result;
        }

        private void ProcessStatus(int status)
        {
            switch (status)
            {
                case 401:
                    throw new AuthenticationException("Wrong api key or your account is suspended");
                case 429:
                    throw new RateLimitException("For FREE plan, exceeding the quota limit; or for all plans, sending too many requests too fast and exceeding the rate limit - https://www.listennotes.com/podcast-api/faq/#faq17");
                case 404:
                    throw new NotFoundException("Endpoint not exist, or podcast / episode not exist.");
                case 400:
                    throw new InvalidRequestException("Something wrong on your end (client side errors), e.g., missing required parameters.");
                case 0:
                    throw new ApiConnectionException("Failed to connect to Listen API servers.");
            }

            if (status >= 500)
            {
                throw new ListenApiException("Error on our end (unexpected server errors)");
            }
        }
    }
}
