using UnityEngine;

namespace AstrotypeInspector
{
    public readonly struct Result
    {
        public readonly bool IsSuccess { get; }
        public readonly Error? Error { get; }
        
        public readonly bool IsFailure => !IsSuccess;
        public readonly string ErrorMessage => Error?.Description;
        
        private Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }
        
        public static Result Success() => new(true, default);
        public static Result Failure(Error error) => new(false, error);
        
        public static implicit operator Result(Error error) => Failure(error);
    }
    
    public readonly struct Result<TValue>
    {
        public readonly bool IsSuccess { get; }
        public readonly Error? Error { get; }
        public readonly TValue Value { get; }
        
        public readonly bool IsFailure => !IsSuccess;
        public readonly string ErrorMessage => Error?.Description;
        
        private Result(bool isSuccess, Error error, TValue value)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
        }
        
        public static Result<TValue> Success(TValue value) => new(true, default, value);
        public static Result<TValue> Failure(Error error) => new(false, error, default);
        
        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(Error error) => Failure(error);
    }
    
    public readonly struct Error
    {
        public readonly string Description { get; }
        
        private Error(string description)
        {
            Description = description;
        }
        
        public static Error Message(string description) => new(description);
    }
    
}
