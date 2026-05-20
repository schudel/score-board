// import {Xliff} from '@angular/compiler';
const fs = require('fs');
const path = require('path');
const localeId = process.argv[2];
if (localeId === undefined) {
    throw new Error(`No language specified.\nUsage: node ${path.basename(__filename)} <locale-id>`);
}
const content = fs.readFileSync(`src/locale/locale.${localeId}.xlf`, 'utf8');
// const xliff = new Xliff().load(content, '');
const i18nServiceFilePath = 'src/app/services/common/i18n.service.ts';
fs.writeFileSync(i18nServiceFilePath, fs.readFileSync(i18nServiceFilePath, 'utf8')
    .replace(/(raw-loader!\.\.\/\.\.\/\.\.\/locale\/locale\.).{5}(\.xlf)/, `$1${localeId}$2`));
