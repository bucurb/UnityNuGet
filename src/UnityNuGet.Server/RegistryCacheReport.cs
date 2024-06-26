﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace UnityNuGet.Server
{
    /// <summary>
    /// Main class that stores relevant messages that may appear during the build of the Unity packages with <see cref="RegistryCache"/>.
    /// </summary>
    public class RegistryCacheReport
    {
        private readonly IList<string> _informationMessages = new List<string>();
        private readonly IList<string> _warningMessages = new List<string>();
        private readonly IList<string> _errorMessages = new List<string>();

        private DateTime? _lastUpdate;

        private readonly RegistryCacheSingleton _registryCacheSingleton;
        private readonly RegistryOptions _registryOptions;

        public RegistryCacheReport(RegistryCacheSingleton registryCacheSingleton, IOptions<RegistryOptions> registryOptionsAccessor)
        {
            _registryCacheSingleton = registryCacheSingleton;
            _registryOptions = registryOptionsAccessor.Value;
        }

        public IEnumerable<string> InformationMeessages => _informationMessages;

        public IEnumerable<string> WarningMessages => _warningMessages;

        public IEnumerable<string> ErrorMessages => _errorMessages;

        public bool Running { get; private set; }

        public double Progress
        {
            get
            {
                var currentIndex = _registryCacheSingleton.ProgressPackageIndex;
                var totalCount = _registryCacheSingleton.ProgressTotalPackageCount;
                var percent = totalCount != 0 ? (double)currentIndex * 100 / totalCount : 0;

                return percent;
            }
        }

        public TimeSpan? TimeRemainingForNextUpdate
        {
            get
            {
                if (_errorMessages.Count == 0)
                {
                    return _lastUpdate != null ? _lastUpdate.Value.Add(_registryOptions.UpdateInterval) - DateTime.UtcNow : null;
                }
                else
                {
                    return TimeSpan.FromSeconds(0);
                }
            }
        }

        public void AddInformation(string message) => _informationMessages.Add(message);

        public void AddWarning(string message) => _warningMessages.Add(message);

        public void AddError(string message) => _errorMessages.Add(message);

        public void Start()
        {
            Running = true;

            _informationMessages.Clear();
            _warningMessages.Clear();
            _errorMessages.Clear();
        }

        public void Complete()
        {
            Running = false;

            _lastUpdate = DateTime.UtcNow;
        }
    }
}
