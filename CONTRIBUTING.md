# Contributing to ChiVariate

This project is a passion project maintained by a single developer with limited time for general maintenance tasks.

However, **performance optimizations** and **documentation improvements** are a top priority and will receive prompt and focused support.

Priority contribution areas include:
- **Bug reports and correctness issues** - pointing out statistical or implementation errors, ideally with suggested fixes
- **Performance optimizations** for existing hot-path code, especially zero-allocation improvements  
- **Platform-specific optimizations** for different .NET runtimes or hardware architectures
- **Documentation improvements** for complex statistical concepts, API usage patterns, or benchmarking guides
- **Testing enhancements** including edge case coverage or statistical validation tests

**Note:** The library intentionally focuses on essential, widely-used distributions. More esoteric distribution requests will generally not be accepted, as the library's extensible design allows users to implement custom distributions using the existing `IChiRngSource<T>` infrastructure.

For other types of contributions (e.g. API design changes, major refactors, general feature requests), please understand that review and response times may vary significantly. If you're considering any substantial change, please open an issue first to discuss the approach before submitting a pull request.

## Performance Contributions

When proposing performance optimizations, please include:
- Benchmarks demonstrating the improvement using BenchmarkDotNet
- Verification that statistical correctness is maintained or improved
- Testing across different .NET runtimes where applicable

## Code Standards

All code must maintain the library's zero- or minimal-allocation design principles. Use `ref struct` patterns where appropriate and ensure compatibility with the `IChiRngSource<T>` abstraction.

All contributions must be licensed under the MIT License. By submitting, you affirm that you have the rights to contribute and agree to license your work accordingly. Please ensure all project-related communication is respectful and constructive.