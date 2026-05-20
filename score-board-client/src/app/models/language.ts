enum LanguageEnum {
  English = 'English',
  German = 'Deutsch',
  SwissGerman = 'Schwyzerdütsch'
}

export class Language {
  private constructor(languageCode: string, languageEnum: LanguageEnum, displayName: string, image: string) {
    this.languageCode = languageCode;
    this.languageEnum = languageEnum;
    this.displayName = displayName;
    this.image = image;
    Language.languages.push(this);
  }
  private static languages: Array<Language> = new Array<Language>();

  public static  English = new Language('en-us', LanguageEnum.English, 'English', 'usa.png');
  public static  German = new Language('de-de', LanguageEnum.German, 'Deutsch', 'germany.png');
  public static  SwissGerman = new Language('de-ch', LanguageEnum.SwissGerman, 'Schwyzerdütsch', 'switzerland.png');

  languageCode: string;
  languageEnum: LanguageEnum;
  displayName: string;
  image: string;

  static getLanguages(): Array<Language> {
    return Language.languages;
  }
}
