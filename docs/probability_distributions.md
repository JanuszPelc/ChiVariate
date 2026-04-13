# Supported probability distributions

The performance of each distribution is grouped into five tiers, based on its relative cost compared to the `Uniform (Continuous)` sampler for `double` precision (~30 ns per sample on Apple M1 Pro).

| Tier | Cost | Description |
|:----:|:----:|-------------|
| 1 | 1-2.5x | Simple sampling and highly efficient algorithms. |
| 2 | 2.5-6x | Common composite distributions. |
| 3 | 6-15.5x | More complex calculations or compositions. |
| 4 | 15.5-39x | Specialized algorithms with significant computational cost. |
| 5 | 39x+ | Inherently high-cost algorithms (e.g., many trials). |

The following probability distributions are supported out of the box, with statistically robust generators and predictable performance costs across `float`, `double`, and high-quality `decimal` variates.

## Discrete distributions

Distributions that produce integer-valued outcomes. Useful for modeling countable events, selections, or success/failure trials.

### Bernoulli

> A single trial that results in 1 (success) or 0 (failure). The fundamental building block for modeling coin flips, critical hits, or any binary yes/no decision with a known probability.
>
>  *Complexity: `O(1)`. Tier 1.*

### Binomial

> The number of successes in a fixed number of independent trials. Excels at counting scenarios like shots hitting a target, customers making purchases, or applications in quality control and A/B testing.
>
>  *Complexity: `O(n)`, where n is the number of trials. Tier 5.*

### Categorical

> A single trial that selects an outcome from a set of discrete categories, each with a different weight. The go-to choice for weighted loot tables, entity spawning, or any selection from multiple options with different likelihoods.
>
>  *Complexity: `O(1)` per sample, with `O(k)` setup where k is the number of categories. Tier 1.*

### Geometric

> The number of trials needed to achieve the first success. Naturally models "how many attempts until success" scenarios, such as attacks needed to land a critical hit or API calls required to get a successful response.
>
>  *Complexity: `O(1/p)`, where p is the success probability. Tier 2.*

### Hypergeometric

> The number of successes in a sample drawn without replacement from a finite population. The foundational distribution for card games (e.g., drawing a 5-card hand), lottery systems, or quality control where items are not returned to the population.
>
>  *Complexity: `O(n)`, where n is the sample size. Tier 4.*

### Multinomial

> The number of outcomes in each of several categories after a fixed number of independent trials. It generalizes the Binomial distribution to multiple categories and handles modeling dice rolls, sorting items into bins, or simulating survey results.
>
>  *Complexity: `O(k+n)`, where k is the number of categories and n is the number of trials. Tier 5.*

### Negative Binomial

> The number of trials required to achieve a fixed number of successes. Designed for scenarios like customer acquisition (how many contacts to get 10 sales), reliability testing, or game mechanics with accumulating successes.
>
>  *Complexity: `O(r/p)`, where r is the number of successes and p is the success probability. Tier 4.*

### Poisson

> The number of events occurring in a fixed interval of time or space at a steady average rate. Excels at modeling counts of "bursty" or rare events, such as emails arriving per minute, customers visiting a store per hour, or random encounters in a game.
>
>  *Complexity: `O(λ)`, where λ (lambda) is the mean. Tier 3.*

### Uniform (Discrete)

> An integer where every value in a given range has an equal chance of being selected. As the fundamental distribution for unbiased choice, it provides the foundation for dice rolls, random array indexing, or any scenario requiring fair selection from a set of options.
>
>  *Complexity: `O(1)`. Tier 1.*

### Zipf

> A distribution where frequency is inversely proportional to rank, modeling "power-law" or "long-tail" phenomena where a few items are common and most are rare. Ideal for simulating word frequencies, city populations, or user engagement patterns.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

## Continuous distributions

Distributions that produce real-valued variates, including `float`, `double`, `Half`, `decimal`, and `ChiFixed` types. Suitable for modeling measurements, durations, or naturally varying quantities.

### Beta

> A random value between 0 and 1 used to represent an unknown probability or proportion. The natural choice for modeling uncertainty about percentages, such as estimating a player's skill from match history or modeling task completion rates.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Cauchy

> A bell-shaped curve with very heavy tails, leading to frequent extreme outliers. Excels at creating dramatic visual effects in particle systems, simulating financial market volatility, or modeling physical resonance and systems prone to extreme events.
>
>  *Complexity: `O(1)`. Tier 1.*

### Chi

> The magnitude (Euclidean distance from the origin) of `k` independent standard normal variables. Commonly applied to calculating signal strength from I/Q components, error distances in targeting systems, or the speed of a particle with random velocity components.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Chi-Squared

> The sum of the squares of `k` independent standard normal variables. As a cornerstone of statistical hypothesis testing, it provides the foundation for goodness-of-fit tests, feature selection in machine learning, and modeling the variance of a sample.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Exponential

> The time between events in a Poisson process. Its "memoryless" property makes it the go-to distribution for modeling waiting times, such as intervals between customer arrivals, time until component failure, or delays before enemy attacks.
>
>  *Complexity: `O(1)`. Tier 1.*

### F

> The ratio of two scaled chi-squared variables. A cornerstone of hypothesis testing, particularly in Analysis of Variance (ANOVA), it specializes in comparing variances between two or more groups to determine if their means are significantly different.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Gamma

> A flexible distribution for positive, skewed values that generalizes the Exponential and Chi-Squared distributions. Well-suited to modeling the total time to complete multiple tasks, the size of insurance claims, or rainfall amounts. Its shape can be tuned from exponential-like to nearly normal.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Gumbel

> An extreme value distribution used to model the maximum (or minimum) of a set of samples. The standard choice in risk management and engineering for events like maximum floods or wind speeds, and foundational to the Gumbel-Max trick in Machine Learning.
>
>  *Complexity: `O(1)`. Tier 1.*

### Inverse-Gamma

> The reciprocal of a Gamma-distributed variable. A cornerstone of Bayesian statistics, it serves as the standard conjugate prior for the unknown variance of a Normal distribution, making it indispensable for modeling uncertainty in variance parameters.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Laplace

> A symmetric, "pointy" distribution (also known as the "double exponential") that is more robust to outliers than the Normal. Particularly effective in robust regression, signal processing, and any model where errors have occasional large deviations.
>
>  *Complexity: `O(1)`. Tier 1.*

### Logistic

> A bell-shaped curve similar to the Normal distribution but with heavier tails. Forms the mathematical basis for logistic regression in machine learning and excels at modeling population growth or any process that follows an S-shaped (sigmoid) curve.
>
>  *Complexity: `O(1)`. Tier 1.*

### Log-Normal

> A continuous distribution of a positive random variable whose logarithm is normally distributed. Designed for phenomena arising from multiplicative processes of many small factors, such as stock prices, income levels, internet traffic, or biological growth rates.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Normal

> The classic symmetric, bell-shaped curve (also known as the Gaussian distribution). The workhorse distribution for modeling a vast range of phenomena from measurement errors to test scores, and for generating realistic noise in simulations.
>
>  *Complexity: Amortized `O(1)`. Tier 1.*

### Pareto

> A skewed distribution that models the "80/20" rule, where a small number of events accounts for a large effect. The quintessential choice for "winner-take-all" phenomena like wealth distribution, city populations, or item popularity.
>
>  *Complexity: `O(1)`. Tier 1.*

### Rayleigh

> The magnitude of a 2D vector whose components are independent, zero-mean normal variables. A special case of the Chi distribution (k=2), it naturally models wind speed, wave heights, or the effect of multi-path signal fading.
>
>  *Complexity: `O(1)`. Tier 1.*

### Student's t

> A bell-shaped curve similar to the Normal but with heavier tails, making it more robust to outliers. The preferred tool for statistical inference, especially when sample sizes are small or the population variance is unknown.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Triangular

> A continuous distribution defined by a minimum, maximum, and most likely (mode) value. Designed for simple modeling when you only have expert estimates for the bounds and the most likely outcome, such as in project management or risk analysis.
>
>  *Complexity: `O(1)`. Tier 1.*

### Uniform (Continuous)

> A real number where every value in a given range has an equal chance of being selected. As the most fundamental continuous distribution, it provides the building blocks for generating other random variates and modeling any scenario where all outcomes in a range are equally likely.
>
>  *Complexity: `O(1)`. Tier 1 (Baseline).*

### Weibull

> A flexible distribution used to model time-to-failure or survival data. The backbone of reliability engineering, its shape parameter allows it to model systems where the failure rate is decreasing (infant mortality), constant (random failures), or increasing (wear-out) over time.
>
>  *Complexity: `O(1)`. Tier 1.*

## Multivariate distributions 

Distributions that produce structured outputs such as vectors or matrices of correlated variates. Used for modeling interdependent random variables and higher-dimensional phenomena.

### Dirichlet

> A vector of probabilities that sum to 1, representing the multivariate generalization of the Beta distribution. The go-to distribution for generating sets of related proportions, such as in topic modeling for text analysis, population genetics, or resource allocation in strategy games.
>
>  *Complexity: `O(k)` per sample, where k is the number of categories. Tier 4.*

### Multivariate Normal

> A vector of correlated random variables, where each variable is normally distributed. As the multivariate generalization of the Normal distribution, it provides the foundation for realistic physics simulations with coupled variables, financial portfolio modeling, or any scenario where multiple random factors influence each other.
>
>  *Complexity: `O(k²)` per sample, plus `O(k³)` setup, where k is the number of dimensions. Tier 3.*

### Wishart

> A random, symmetric, positive-semidefinite matrix that represents the multivariate generalization of the Chi-Squared distribution. A cornerstone of multivariate Bayesian analysis, it specializes in generating random covariance matrices, allowing you to model uncertainty in the relationships between multiple variables.
>
>  *Complexity: `O(k³)` per sample, plus `O(k³)` setup, where k is the number of dimensions. Tier 4.*