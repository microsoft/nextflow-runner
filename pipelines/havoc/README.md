# nf-HAVoC (Helsinki university Analyzer for Variants of Concern)

## Description
**nf-HAVoC** is a bioinformatic pipeline designed to provide an accessible tool for constructing consensus sequences from SARS-CoV-2 FASTQ files and identifying the variants they belong to. The pipeline is a modification of HAVoC (DOI: https://doi.org/10.1186/s12859-021-04294-2) and built on [Nextflow](https://www.nextflow.io), a workflow tool to run tasks across multiple compute infrastructures in a very portable manner. It uses Docker/Singularity containers making installation trivial and results highly reproducible.




## Pipeline summary

nf-HAVoC can be run to analyze FASTQ files within a directory/

The target directory must contain matching FASTQ files for forward (R1) and reverse (R2) reads, which can be either gzipped (\*.fastq.gz) or uncompressed (\*.fastq).


The following options can be changed as commandline options  depending on your preferences:

| Option        | Input                 | Function                                                    |
| :------------ | :-------------------- | :---------------------------------------------------------- |
| prepro  | fastp* or trimmomatic | Tool for pre-processing FASTQ files.                        |
| aligner | bowtie or bwa*        | Tool for aligning reads.                                    |
| sam     | sambamba* or samtools | Tool for SAM/BAM processing                                 |
| coverage  | Number (30*)          | Minimum coverage for masking regions in consensus sequence. |
| pangolin  | yes* or no            | Run Pangolin for lineage identification.                    |

\* By default.

**Only one tool can be utilized per option.**


## Quick Start

1. Install [`Nextflow`](https://www.nextflow.io/docs/latest/getstarted.html#installation) (`>=20.10.0`)

2. Install any of [`Docker`](https://docs.docker.com/engine/installation/)

3. Download [test data and adapter and reference files](https://bitbucket.org/auto_cov_pipeline/havoc)

3. Download the pipeline and test it on a minimal dataset with a single command:

    ```console
    nextflow run microsoft/nf-HAVoC -profile local --nextera 'path/to/NexteraPE-PE.fa' --ref path/to/ref.fa' --reads 'path/to/fastq/*R{1,2}*fastq.gz' =
    ```


4. Start running your own analysis on Azure.

Azure Batch Setup

Please refer to the [Nextflow](https://www.nextflow.io/docs/edge/azure.html) documentation which describe how to setup the Azure Batch environment.


    ```console
     nextflow run microsoft/nf-HAVoC -profile azure --nextera 'az://coantiner/NexteraPE-PE.fa' --ref 'az://container/ref.fa' --reads 'az://container/*R{1,2}*fastq.gz' -w 'az://conatiner/work'
    ```

    * Azure speicifcs can be changed in the config locally or provided as commandline options*


## Credits

These scripts were originally found [Original Code](https://bitbucket.org/auto_cov_pipeline/havoc). For more information visit Webpage [Helsinki university Analyzer for Variants of Concern](https://www.helsinki.fi/en/projects/havoc).


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
