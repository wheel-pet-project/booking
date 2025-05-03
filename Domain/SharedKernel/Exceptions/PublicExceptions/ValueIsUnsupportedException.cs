using System.Diagnostics.CodeAnalysis;

namespace Domain.SharedKernel.Exceptions.PublicExceptions;

[ExcludeFromCodeCoverage]
public class ValueIsUnsupportedException(string message = "Character out of range") : PublicException(message);