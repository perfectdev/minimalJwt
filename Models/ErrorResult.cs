namespace MinimalJwt.Models; 

public record ErrorResult(string errorKey, string errorMessage, string localizationKey);