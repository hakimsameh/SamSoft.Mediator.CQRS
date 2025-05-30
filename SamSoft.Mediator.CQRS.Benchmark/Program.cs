using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using MediatR;
using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.Benchmark;


BenchmarkRunner.Run<MediatorBenchmarks>();

// SamSoft.Mediator.CQRS command/handler
