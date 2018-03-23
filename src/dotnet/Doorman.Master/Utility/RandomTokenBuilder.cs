using System;
using MlkPwgen;

namespace Doorman.Master.Utility {
    class RandomTokenBuilder {
        private const string ALLOWED_CHARS = Sets.Alphanumerics;
        private const int DEFAULT_LENGTH = 64;

        private readonly int _length;

        public RandomTokenBuilder(int length = DEFAULT_LENGTH) {
            this._length = length;
        }   

        public string GenerateToken() {
            return PasswordGenerator.Generate(this._length, ALLOWED_CHARS);
        } 
    }
}