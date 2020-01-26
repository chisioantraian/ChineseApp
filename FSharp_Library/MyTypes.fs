module MyTypes

type DetailedWord = {
    Simplified: string
    Length: string
    Pinyin: string
    PinyinInput: string
    WCount: string
    WMillion: string
    Log10W: string
    W_CD: string
    W_CD_percent: string
    Log10CD: string
    DominantPos: string
    DominantPosFreq: string
    AllPos: string
    AllPosFreq: string
    Definition: string
}

type Word = {
    mutable Rank: int
    Traditional: string
    Simplified: string
    Pronounciation: string
    Definitions: string
}