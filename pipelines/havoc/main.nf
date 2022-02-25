#!/usr/bin/env nextflow


log.info """\
 H A V o C -   P I P E L I N E
 ===================================
 reads      : ${params.reads}
 nextera    : ${params.nextera}
 ref        : ${params.ref}
 outdir     : ${params.outdir}
 prepro     : ${params.prepro}
 aligner    : ${params.aligner}
 sam        : ${params.sam}
 coverage   : ${params.coverage}
 pangolin   : ${params.pangolin}
 """


 /*
  * Create the `read_pairs_ch` channel that emits tuples containing three elements:
  * the pair ID, the first read-pair file and the second read-pair file
  */
 Channel
     .fromFilePairs( params.reads )
     .ifEmpty { error "Cannot find any reads matching: ${params.reads}" }
     .set { read_pairs_ch }

   // Check reference files exist
  if (params.ref) {
      ref = file(params.ref, checkIfExists: true)
      if (ref.isEmpty()) {exit 1, "Reference file provided is empty : ${ch_ribo_db.getName()}!"}
  }

  // Check adapter files exist
  if (params.nextera) {
      nextera = file(params.nextera, checkIfExists: true)
      if (nextera.isEmpty()) {exit 1, "Adapter file provided is empty : ${ch_ribo_db.getName()}!"}
  }

process runHavoc {
  tag "$pair_id"
  publishDir "$outDir/$pair_id", mode: 'copy'

	input:
	path nextera from nextera
	path ref from ref
	tuple val(pair_id), path(reads) from read_pairs_ch

  output:
  path '*bam'
  path '*vcf'
  path '*_consensus.fa'
  path '*_R*fastq*'
  path '*_lowcovmask.bed'
  path 'fastp.*'
  path 'pangolearn_assignments.csv'

	"""
  bash HAVoC.sh -n $nextera -r $ref -p $params.prepro -a $params.aligner -s $params.sam -m $params.coverage  -o $params.pangolin $reads
	"""
}
